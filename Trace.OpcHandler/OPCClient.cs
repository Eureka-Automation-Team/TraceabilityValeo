// By Andrew Hassett
// Last updated 29 August 2019

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;

namespace OPCUserInterface
{
	// OPC variables types:
	public enum OPCVarType
	{
		// These are the variable (tag) types for an Allen Bradley Logix PLC:
		BOOL,  // C# bool, 1 byte
		SINT,  // C# byte, 1 byte
		INT,   // C# short, 2 bytes
		DINT,  // C# int, 4 bytes
		REAL,  // C# float, 4 bytes
		STRING, // C# text
	}

	// -------------------------------------------------------------------------
	// Public input data for each variable:
	public class OPCVar
	{
		public string RefName { get; set; } // Name to refer to the variable in the application
		public string Name { get; set; } // OPC variable name - not including topic (e.g. "Program:MainProgram.TestCount")
		public OPCVarType VarType { get; set; } // OPC variable type (not C# type!)

		public OPCVar(string RefNameIn, string NameIn, OPCVarType VarTypeIn)
		{
			RefName = RefNameIn;
			Name = NameIn;
			VarType = VarTypeIn;
		}
	}

	// ---------------------------------------------------------------------------
	// Class to store data from notification
	class OPCNotificationDataClass
	{
		// Note - All Logix variables are stored in 4 bytes
		public bool Nbool { get; set; } // AB BOOL, C# bool
		public sbyte Nsbyte { get; set; } // AB SINT, C# byte, 1 byte
		public short Nshort { get; set; } // AB INT, C# short, 2 bytes
		public int Nint { get; set; } // AB DINT, C# int, 4 bytes
		public float Nfloat { get; set; } // AB REAL, C# float, 4 bytes
		public string Nstring { get; set; }
	}

	// ---------------------------------------------------------------------------
	public class OPCClient
	{
		public Action NotificationHandler; // Subscribe to this Action for notification handling

		//public Action ReadHandler; // Subscribe to this Action for read variable handling

		public Action<string> ComErrorHandler; // Subscribe to this Action to show com error messages

		// Private information for each OPC variable:
		private class OPCVarData
		{
			public string Name { get; set; } // OPC variable name (e.g. "Program:MainProgram.TestCount")
			public OPCVarType VarType { get; set; } // OPC variable type (not C# type!)
			public bool NotificationReceived { get; set; } // true if a value change notification has been recieved
			public OPCNotificationDataClass NotificationData = new OPCNotificationDataClass();
			public int ItemEventIndex { get; set; }
			public string FullName { get; set; }
		}

		// Store data on variables in a dictionary for quick access:
		private Dictionary<string, OPCVarData> OPCEventVars;
		private Dictionary<string, OPCVarData> OPCWriteVars;

		private Opc.URL Url;  // For RSLinx local OPC server Url = "opcda://localhost/RSLinx OPC Server"
		private Opc.Da.Server Server;
		private OpcCom.Factory Factory = new OpcCom.Factory();
		private string OPCTopic; // DDE/OPC Topic name created in RSLinx

		// Group for writing variables:
		private Opc.Da.Subscription groupWrite;
		private Opc.Da.SubscriptionState groupStateWrite;

		// Group for event notifications:
		private Opc.Da.Subscription groupEvents;
		private Opc.Da.SubscriptionState groupStateEvents;
		private int NumEventVars = 0;

		private bool Connected = false; // True if connection was successful

		//---------------------------------------------------------------------
		// Initialise Communication:

		public bool Init(List<OPCVar> OPCEventVarListIn, List<OPCVar> OPCWriteVarListIn, string UrlIn, string OPCTopicIn)
		{
			//Connect to the OPC server:
			Connected = false;
			OPCTopic = OPCTopicIn;

			//Build dictionary of event variable data:
			OPCEventVars = new Dictionary<string, OPCVarData>();
			NumEventVars = 0;
			foreach (OPCVar OPCVarIn in OPCEventVarListIn)
			{
				OPCEventVars.Add(OPCVarIn.RefName, new OPCVarData());

				OPCEventVars[OPCVarIn.RefName].Name = OPCVarIn.Name;
				OPCEventVars[OPCVarIn.RefName].VarType = OPCVarIn.VarType;
				OPCEventVars[OPCVarIn.RefName].NotificationReceived = false;
				OPCEventVars[OPCVarIn.RefName].ItemEventIndex = 0;
				OPCEventVars[OPCVarIn.RefName].FullName = "[" + OPCTopic + "]" + OPCVarIn.Name;

				NumEventVars++;
			} // End foreach

			//Build dictionary of write variable data:
			OPCWriteVars = new Dictionary<string, OPCVarData>();
			foreach (OPCVar OPCVarIn in OPCWriteVarListIn)
			{
				OPCWriteVars.Add(OPCVarIn.RefName, new OPCVarData());

				OPCWriteVars[OPCVarIn.RefName].Name = OPCVarIn.Name;
				OPCWriteVars[OPCVarIn.RefName].VarType = OPCVarIn.VarType;
				OPCWriteVars[OPCVarIn.RefName].FullName = "[" + OPCTopic + "]" + OPCVarIn.Name;

			} // End foreach

			try
			{
				// Connect to OPC server:
				Url = new Opc.URL(UrlIn);
				Server = new Opc.Da.Server(Factory, null);
				Server.Connect(Url, new Opc.ConnectData(new System.Net.NetworkCredential()));

				// Create a write group:            
				groupStateWrite = new Opc.Da.SubscriptionState();
				groupStateWrite.Name = "Group Write";
				groupStateWrite.Active = false; //not needed to read if you want to write only
				groupWrite = (Opc.Da.Subscription)Server.CreateSubscription(groupStateWrite);

				// Create an event group:
				groupStateEvents = new Opc.Da.SubscriptionState();
				groupStateEvents.Name = "Group Events";
				groupStateEvents.Active = true;
				groupEvents = (Opc.Da.Subscription)Server.CreateSubscription(groupStateEvents);

				// Add items to the event group:
				Opc.Da.Item[] itemsEvents = new Opc.Da.Item[NumEventVars];
				int j = 0;
				foreach (OPCVar OPCVarIn in OPCEventVarListIn)
				{
					OPCEventVars[OPCVarIn.RefName].ItemEventIndex = j;
					itemsEvents[j] = new Opc.Da.Item();
					itemsEvents[j].ItemName = OPCEventVars[OPCVarIn.RefName].FullName;
					j++;
				}

				itemsEvents = groupEvents.AddItems(itemsEvents);
				groupEvents.DataChanged += new Opc.Da.DataChangedEventHandler(OnTransactionCompleted);

				Connected = true;
				return true;
			}
			catch
			{
				return false;
			}
		}

		// --------------------------------------------------------------------------

		public bool IsConnected()
		{
			return Connected;
		}

		// --------------------------------------------------------------------------
		public bool WriteVar<T>(string RefNameIn, T Value)
		{
			if (!Connected) return false;

			try
			{
				//Remove all existing items from the group
				groupWrite.RemoveItems(groupWrite.Items);

				//Create the item to write:
				Opc.Da.Item[] itemToAdd = new Opc.Da.Item[1];
				itemToAdd[0] = new Opc.Da.Item();
				itemToAdd[0].ItemName = OPCWriteVars[RefNameIn].FullName;

				//create the item that contains the value to write
				Opc.Da.ItemValue[] writeValues = new Opc.Da.ItemValue[1];
				writeValues[0] = new Opc.Da.ItemValue(itemToAdd[0]);

				groupWrite.AddItems(itemToAdd);
				writeValues[0].ServerHandle = groupWrite.Items[groupWrite.Items.Length - 1].ServerHandle;

				//set the value to write
				switch (OPCWriteVars[RefNameIn].VarType)
				{
					case OPCVarType.BOOL:
						writeValues[0].Value = Convert.ToBoolean(Value);
						break;
					case OPCVarType.SINT:
						writeValues[0].Value = Convert.ToSByte(Value);
						break;
					case OPCVarType.INT:
						writeValues[0].Value = Convert.ToInt16(Value);
						break;
					case OPCVarType.DINT:
						writeValues[0].Value = Convert.ToInt32(Value);
						break;
					case OPCVarType.REAL:
						writeValues[0].Value = Convert.ToSingle(Value);
						break;
					case OPCVarType.STRING:
						writeValues[0].Value = Convert.ToString(Value);
						break;
				}

				//write
				groupWrite.Write(writeValues);

				return true;
			}
			catch
			{
				ComError("Write to OPC variable failed");
				return false;
			}
		}

		// --------------------------------------------------------------------
		// Event notification handling:		

		private delegate void OnTransactionCompletedDelegate(object group, object hReq, Opc.Da.ItemValueResult[] items);

		private void OnTransactionCompleted(object group, object hReq, Opc.Da.ItemValueResult[] items)
		{
			try
			{
				string keyName;

				for (int i = 0; i < items.GetLength(0); i++)
				{
					keyName = "";
					foreach (KeyValuePair<string, OPCVarData> pair in OPCEventVars)
					{
						if (pair.Value.FullName == items[i].ItemName)
						{
							keyName = pair.Key;
							break;
						}
					}
					if (keyName == "") break; // Key not found

					OPCEventVars[keyName].NotificationReceived = true;

					switch (OPCEventVars[keyName].VarType)
					{
						case OPCVarType.BOOL:
							OPCEventVars[keyName].NotificationData.Nbool = Convert.ToBoolean(items[i].Value);
							break;
						case OPCVarType.SINT:
							OPCEventVars[keyName].NotificationData.Nsbyte = Convert.ToSByte(items[i].Value);
							break;
						case OPCVarType.INT:
							OPCEventVars[keyName].NotificationData.Nshort = Convert.ToInt16(items[i].Value);
							break;
						case OPCVarType.DINT:
							OPCEventVars[keyName].NotificationData.Nint = Convert.ToInt32(items[i].Value);
							break;
						case OPCVarType.REAL:
							OPCEventVars[keyName].NotificationData.Nfloat = Convert.ToSingle(items[i].Value);
							break;
						case OPCVarType.STRING:
							OPCEventVars[keyName].NotificationData.Nstring = Convert.ToString(items[i].Value);
							break;
					}
				}

				// call notification handler:
				if (NotificationHandler != null) NotificationHandler();
			}
			catch
			{
				ComError("Event Handling Method Failed");
			}

		}

		// --------------------------------------------------------------------------
		// Methods to return notification data:

		public bool GetNotifiedBOOL(string VarName) { return OPCEventVars[VarName].NotificationData.Nbool; } // returns C# bool
		public sbyte GetNotifiedSINT(string VarName) { return OPCEventVars[VarName].NotificationData.Nsbyte; } // returns C# sbyte
		public short GetNotifiedINT(string VarName) { return OPCEventVars[VarName].NotificationData.Nshort; } // returns C# short
		public int GetNotifiedDINT(string VarName) { return OPCEventVars[VarName].NotificationData.Nint; } // returns C# int
		public float GetNotifiedREAL(string VarName) { return OPCEventVars[VarName].NotificationData.Nfloat; } // returns C# float
		public string GetNotifiedSTRING(string VarName) { return OPCEventVars[VarName].NotificationData.Nstring; } // returns C# string

		// --------------------------------------------------------------------

		public void ClearNotification(string VarName)
		{
			OPCEventVars[VarName].NotificationReceived = false;
		}

		// --------------------------------------------------------------------

		public bool GetNotificationReceived(string VarName)
		{
			bool b;
			b = OPCEventVars[VarName].NotificationReceived;
			OPCEventVars[VarName].NotificationReceived = false; // Clear notification received
			return b;
		}

		// --------------------------------------------------------------------
		private void ComError(string ErrorMessage)
		{
			if (ComErrorHandler != null) ComErrorHandler(ErrorMessage);

		}

	} // End class OPCClient

}
