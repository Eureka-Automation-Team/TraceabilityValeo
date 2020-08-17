using Opc.Da;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trace.Data;
using Trace.Data.Service;
using Trace.Domain.Models;
using Trace.Domain.Services;

namespace Trace.OpcHandlerMachine05.Presenters
{
    public class MainPresenter
    {
        IDataService<MachineModel> _serviceMachine = new MachineService(new TraceDbContextFactory());
        IDataService<PlcTagModel> _servicePLCTag = new PLCTagService(new TraceDbContextFactory());
        IDataService<TraceabilityLogModel> _serviceTraceLog = new TraceabilityLogService(new TraceDbContextFactory());
        IDataService<TighteningResultModel> _serviceTigthening = new TighteningResultService(new TraceDbContextFactory());
        IDataService<TighteningRepairModel> _serviceTigtheningRepair = new TighteningRepairService(new TraceDbContextFactory());
        IDataService<CameraResultModel> _serviceCameraResult = new CameraResultService(new TraceDbContextFactory());
        IDataService<PartAssemblyModel> _servicePartAssembly = new PartAssemblyService(new TraceDbContextFactory());

        private readonly IMonitoringView _view;
        public MainPresenter(IMonitoringView view)
        {
            _view = view;

            _view.FormLoad += Initailization;
            _view.Connect_Click += Connect;
            _view.Disconnect_Click += Disconnect;
            //_view.InterLock += InterLock;
            _view.MakeReady += MakeReady;
            _view.KeepLogging += KeepLogging;
            _view.RefreshData += RefreshData;
            _view.VerityCode += VerityCode;
        }

        private void VerityCode(object sender, EventArgs e)
        {
            MachineModel _machine = sender as MachineModel;
            var result = _view.groupRead.Read(_view.groupRead.Items).ToList();

            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("<<=================== Start time : {0} ===================>>"
                                                                 , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

            var tagName = _view.tagMainBlock + "ST4CodeVerify";
            var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("Get PLC Tag value time : {0}"
                                                                 , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

            var loggings = _serviceTraceLog.GetListByItemCode(value.ToString());
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("Get Item Code : {0} from database time : {1}"
                                                                , value.ToString()
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

            if (loggings.Count() == 0)
            {
                //Data not found
                //_machine.CodeVerifyResult = 3;  
                _machine.CodeVerifyResult = 1;
            }
            else
            {
                //Dupplicated
                //_machine.CodeVerifyResult = 4; 
                _machine.CodeVerifyResult = 2;
            }

            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("Verify Code Result : {0} => Time : {1}", _machine.CodeVerifyResult.ToString()
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

            var reactResult = WriteWord(_view.tagMainBlock + "ST4CodeVerifyResult", _machine.CodeVerifyResult.ToString());
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("Write PLC Tag : {0}  Value = [{2}] => Complete Time : {1}"
                                                                , "ST4CodeVerifyResult"
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
                                                                , reactResult.ToString()));
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("<<=================== End time : {0} ===================>>"
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", "");
            _view.machine = _machine;
        }

        private void RefreshData(object sender, EventArgs e)
        {
            if (_view.connectedPlc)
                LoadCurrentValue(_view.groupRead);
        }

        private void KeepLogging(object sender, EventArgs e)
        {
            if (_view.systemReady)
            {
                MachineModel machine = (MachineModel)sender;
                ItemValueResult[] subscipt;
                try
                {
                    subscipt = _view.groupRead.Read(_view.groupRead.Items);
                }
                catch (Opc.ResultIDException ex)
                {
                    _view.connectedPlc = false;
                    _view.systemReady = false;
                    _view.ResultnMessage = ex.Message;
                    return;
                }

                var result = subscipt.ToList();
                var machineTags = _servicePLCTag.GetAll().ToList().Where(x => x.MachineId == machine.Id);
                var tags = (from tag in machineTags
                            where tag.MachineId == machine.Id
                            select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                #region Station1
                if (machine.RequestLogging)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine;
                    machineTmp.MessageResult = string.Empty;

                    try
                    {
                        WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("<<=================== Start time : {0} ===================>>"
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
                        keepLog = LoggingData(r, machine, machineTags);
                        if (keepLog)
                        {
                            machineTmp.CompletedLogging = 1;
                            WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Insert to Database complete time : {0}"
                                                               , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
                        }
                        else
                            machineTmp.CompletedLogging = 3;
                    }
                    catch (Exception ex)
                    {
                        _view.ResultnMessage = ex.Message;
                        machineTmp.CompletedLogging = 3;
                    }

                    WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Logging Result : {0} => Time : {1}"
                                                                , machineTmp.CompletedLogging.ToString()
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
                    var reactResult = WriteWord(_view.tagMainBlock + "ST4LoggingApp", machineTmp.CompletedLogging.ToString());
                    WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Write PLC Tag : {0}  Value = [{2}] => Complete Time : {1}"
                                                                , "ST4LoggingApp"
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
                                                                , machineTmp.CompletedLogging.ToString()));
                    WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("<<=================== End time : {0} ===================>>"
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
                    WriteLog("KeepLogging" + _view.machine.Id + ".txt", "");
                    _view.machine = machineTmp;
                    LoadCurrentValue(_view.groupRead);
                }

                #endregion
            }
        }

        private bool LoggingData(IEnumerable<ItemValueResult> r, MachineModel machine, IEnumerable<PlcTagModel> machineTags)
        {
            bool result = true;
            bool invalid = false;
            string errMsg = string.Empty;
            string tmpMsg = string.Empty;

            TraceabilityLogModel trace = new TraceabilityLogModel();

            var tagsPart = (from tag in machineTags
                            where tag.TypeCode == "DATA_PART_ASSEMBLY"
                            select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();
            var tagsTightening = (from tag in machineTags
                                  where tag.TypeCode == "DATA_TIGHTENING_RESULT"
                                  select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

            trace.StationId = machine.StationId;
            trace.MachineId = machine.Id;
            trace.ProductionDate = DateTime.Now;

            #region Tightening Keep position #001
            //Tightening Keep position #001
            var tagName = _view.tagMainBlock + "ST4Code";
            var itemCode = r.Where(x => x.ItemName == tagName).FirstOrDefault().Value.ToString();
            WriteLog("KeepLoggingST4.txt", String.Format("Item Code = {0}", itemCode));
            var loggings = _serviceTraceLog.GetListByItemCode(itemCode)
                                            .Where(x =>
                                                   x.MachineId == machine.Id
                                                   //&& x.CreationDate.Date == DateTime.Now.Date
                                                   //&& !x.FinishFlag
                                                   ).ToList();

            //Exit if item code is null
            if (string.IsNullOrEmpty(itemCode))
            {
                return true;
            }

            if (loggings.Count() > 0)
            {
                bool finishFlag = false;
                if (trace.FinalResult != 0)
                    finishFlag = true;

                trace = _serviceTraceLog.GetByID(loggings.FirstOrDefault().Id);
                trace.FinishFlag = finishFlag;
            }
            #endregion Tightening Keep position #001

            foreach (var item in r)
            {
                tmpMsg = string.Empty;
                if (InvalidDataTag((item.Value == null) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }

                if (!invalid)
                {
                    if (item.ItemName == _view.tagMainBlock + "ST4Code")
                        trace.ItemCode = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST4Final_Judgment")
                    {
                        if (Convert.ToInt32(item.Value) != 0)
                        {
                            trace.FinalResult = Convert.ToInt32(item.Value);
                        }
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST4ModelRunning")
                        trace.ModelRunning = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST4ModelRunning2")
                        trace.ModelRunningFlag = Convert.ToInt32(item.Value);
                }
            }

            //Keep part Assemblies
            trace.PartAssemblies = new List<PartAssemblyModel>();
            string partActuaterSN = string.Empty;
            foreach (var item in r.Where(x => tagsPart.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                tmpMsg = string.Empty;
                if (InvalidDataTag((item.Value == null) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }

                if (!invalid)
                {
                    PartAssemblyModel part = new PartAssemblyModel();
                    if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[2]")
                    {
                        part.LineNumber = 3;
                        part.PartName = "LWR Frame";
                        part.SerialNumber = item.Value.ToString();
                    }

                    if (trace.ModelRunningFlag == 1)
                    {
                        if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[0]")
                        {
                            part.LineNumber = 1;
                            part.PartName = "LWR Actuator P/N";
                            part.SerialNumber = item.Value.ToString();
                            if (!string.IsNullOrEmpty(part.SerialNumber))
                                partActuaterSN = part.SerialNumber.Substring(11, (part.SerialNumber.Length - 11));
                        }

                        if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[1]")
                        {
                            part.LineNumber = 2;
                            part.PartName = "LWR Actuator S/N";
                            part.SerialNumber = partActuaterSN;// item.Value.ToString();
                        }

                        if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[3]")
                        {
                            part.LineNumber = 4;
                            part.PartName = "Vane LH1";
                            part.SerialNumber = item.Value.ToString();
                        }

                        if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[4]")
                        {
                            part.LineNumber = 5;
                            part.PartName = "Vane LH2";
                            part.SerialNumber = item.Value.ToString();
                        }

                        if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[5]")
                        {
                            part.LineNumber = 6;
                            part.PartName = "Vane LH3";
                            part.SerialNumber = item.Value.ToString();
                        }

                        if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[6]")
                        {
                            part.LineNumber = 7;
                            part.PartName = "Vane RH1";
                            part.SerialNumber = item.Value.ToString();
                        }

                        if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[7]")
                        {
                            part.LineNumber = 8;
                            part.PartName = "Vane RH2";
                            part.SerialNumber = item.Value.ToString();
                        }

                        if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[8]")
                        {
                            part.LineNumber = 9;
                            part.PartName = "Vane RH3";
                            part.SerialNumber = item.Value.ToString();
                        }

                        if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[9]")
                        {
                            part.LineNumber = 10;
                            part.PartName = "COVER";
                            part.SerialNumber = item.Value.ToString();
                        }

                        if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[10]")
                        {
                            part.LineNumber = 11;
                            part.PartName = "Link Driver";
                            part.SerialNumber = item.Value.ToString();
                        }
                    }
                    
                    if(!string.IsNullOrEmpty(part.PartName))
                        trace.PartAssemblies.Add(part);
                }
            }
           
            //Keep Tightening
            int i = 1;
            List<TighteningRepairModel> tRepairs = new List<TighteningRepairModel>();
            TighteningRepairModel tRepair = new TighteningRepairModel();
            TighteningResultModel tmp = new TighteningResultModel();

            if(trace.ModelRunningFlag == 1)
            {
                if (trace.TighteningResults == null)
                    trace.TighteningResults = new List<TighteningResultModel>();

                foreach (var item in r.Where(x => tagsTightening.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
                {
                    /*
                    tmpMsg = string.Empty;                
                    if (InvalidDataTag((item.Value == null) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                    {
                        invalid = true;
                        if (!string.IsNullOrEmpty(errMsg))
                            errMsg += Environment.NewLine;

                        errMsg += tmpMsg;
                    }*/

                    if (!invalid)
                    {
                        //No.1
                        if (item.ItemName == _view.tagMainBlock + "ST4TestResult[0]")
                        {
                            TighteningResultModel t = new TighteningResultModel();
                            t.No = i.ToString();
                            t.Result = ConvertToDecimal(item.Value.ToString());
                            t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter2[0]").FirstOrDefault().Value.ToString());
                            t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter1[0]").FirstOrDefault().Value.ToString());
                            t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter3[0]").FirstOrDefault().Value.ToString());
                            t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST4TestJudgment[0]").FirstOrDefault().Value.ToString();

                            //JointsControlAngle Result 
                            t.JointResult = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4TestResult[3]").FirstOrDefault().Value.ToString());
                            t.JointMin = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter2[3]").FirstOrDefault().Value.ToString());
                            t.JointMax = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter1[3]").FirstOrDefault().Value.ToString());
                            t.JointTarget = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter3[3]").FirstOrDefault().Value.ToString());
                            t.JointTestResult = t.TestResult; // r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[8]").FirstOrDefault().Value.ToString();

                            tRepair = autoMappingRepair(t);
                            List<TighteningResultModel> tExist = new List<TighteningResultModel>();
                            tExist = trace.TighteningResults.Where(x => x.No == t.No).ToList();
                            if (tExist.Count() == 0)
                            {
                                trace.TighteningResults.Add(t);
                                tRepairs.Add(tRepair);
                            }
                            else if (tExist.Where(x => (string.IsNullOrEmpty(x.TestResult) ? "NOK" : x.TestResult) == "NOK").Count() > 0)
                            {
                                tRepairs.Add(tRepair);
                                trace.TighteningResults.Where(x => x.No == t.No && (string.IsNullOrEmpty(x.TestResult) ? "NOK" : x.TestResult) == "NOK")
                                    .Select(c => {
                                        c.RepairFlag = (c.TestResult == "NOK");
                                        c.Result = t.Result;
                                        c.Min = t.Min;
                                        c.Max = t.Max;
                                        c.Target = t.Target;
                                        c.TestResult = t.TestResult;
                                        c.JointResult = t.JointResult;
                                        c.JointMin = t.JointMin;
                                        c.JointMax = t.JointMax;
                                        c.JointTarget = t.JointTarget;
                                        c.JointTestResult = t.TestResult;
                                        return c;
                                    }).ToList();
                            }
                        }

                        //No.2
                        if (item.ItemName == _view.tagMainBlock + "ST4TestResult[1]")
                        {
                            TighteningResultModel t = new TighteningResultModel();
                            t.No = i.ToString();
                            t.Result = ConvertToDecimal(item.Value.ToString());
                            t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter2[1]").FirstOrDefault().Value.ToString());
                            t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter1[1]").FirstOrDefault().Value.ToString());
                            t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter3[1]").FirstOrDefault().Value.ToString());
                            t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST4TestJudgment[1]").FirstOrDefault().Value.ToString();

                            //JointsControlAngle Result 
                            t.JointResult = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4TestResult[4]").FirstOrDefault().Value.ToString());
                            t.JointMin = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter2[4]").FirstOrDefault().Value.ToString());
                            t.JointMax = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter1[4]").FirstOrDefault().Value.ToString());
                            t.JointTarget = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter3[4]").FirstOrDefault().Value.ToString());
                            t.JointTestResult = t.TestResult; // r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[8]").FirstOrDefault().Value.ToString();

                            tRepair = autoMappingRepair(t);
                            List<TighteningResultModel> tExist = new List<TighteningResultModel>();
                            tExist = trace.TighteningResults.Where(x => x.No == t.No).ToList();
                            if (tExist.Count() == 0)
                            {
                                trace.TighteningResults.Add(t);
                                tRepairs.Add(tRepair);
                            }
                            else if (tExist.Where(x => (string.IsNullOrEmpty(x.TestResult) ? "NOK" : x.TestResult) == "NOK").Count() > 0)
                            {
                                tRepairs.Add(tRepair);
                                trace.TighteningResults.Where(x => x.No == t.No && (string.IsNullOrEmpty(x.TestResult) ? "NOK" : x.TestResult) == "NOK")
                                    .Select(c => {
                                        c.RepairFlag = (c.TestResult == "NOK");
                                        c.Result = t.Result;
                                        c.Min = t.Min;
                                        c.Max = t.Max;
                                        c.Target = t.Target;
                                        c.TestResult = t.TestResult;
                                        c.JointResult = t.JointResult;
                                        c.JointMin = t.JointMin;
                                        c.JointMax = t.JointMax;
                                        c.JointTarget = t.JointTarget;
                                        c.JointTestResult = t.TestResult;
                                        return c;
                                    }).ToList();
                            }
                        }
                    }
                    i++;
                }
            }

            if (invalid)
            {
                var mac = _view.machine;
                mac.MessageResult = errMsg;
                _view.ResultnMessage = mac.MessageResult;
                mac.CompletedLogging = 3;
                _view.machine = mac;
                result = false;
            }
            else
            {
                if (trace.FinalResult != 0)
                    trace.FinishFlag = true;

                if (trace.Id == 0 && !string.IsNullOrEmpty(trace.ItemCode))
                {
                    TraceabilityLogModel logResult = _serviceTraceLog.Create(trace);
                    foreach (var item in logResult.TighteningResults.OrderBy(o => o.No))
                    {
                        var cRepair = tRepairs.Where(x => x.No == item.No).FirstOrDefault();
                        if (cRepair != null)
                        {
                            if (!string.IsNullOrEmpty(cRepair.TestResult))
                            {
                                cRepair.TighteningResultId = item.Id;
                                _serviceTigtheningRepair.Create(cRepair);
                            }
                        }
                    }
                    result = true;
                }
                else if (trace.Id != 0 && !string.IsNullOrEmpty(trace.ItemCode))
                {
                    TraceabilityLogModel logResult = _serviceTraceLog.Update(trace);
                    foreach (var item in logResult.TighteningResults.OrderBy(o => o.No))
                    {
                        TighteningResultModel tigthening = new TighteningResultModel();
                        if (item.Id == 0)
                        {
                            item.TraceLogId = logResult.Id;
                            tigthening = _serviceTigthening.Create(item);
                        }
                        else
                        {
                            tigthening = _serviceTigthening.Update(item);
                        }

                        if (tigthening != null)
                        {
                            var cRepair = tRepairs.Where(x => x.No == item.No).FirstOrDefault();
                            if (cRepair != null)
                            {
                                if (!string.IsNullOrEmpty(cRepair.TestResult))
                                {
                                    cRepair.TighteningResultId = item.Id;
                                    _serviceTigtheningRepair.Create(cRepair);
                                }
                            }
                        }
                    }

                    result = true;
                }
            }

            return result;
        }

        private TighteningRepairModel autoMappingRepair(TighteningResultModel t)
        {
            TighteningRepairModel tr = new TighteningRepairModel();

            tr.No = t.No;
            tr.Result = t.Result;
            tr.Min = t.Min;
            tr.Max = t.Max;
            tr.Target = t.Target;
            tr.TestResult = t.TestResult;

            //JointsControlAngle Result
            tr.JointResult = t.JointResult;
            tr.JointMin = t.JointMin;
            tr.JointMax = t.JointMax;
            tr.JointTarget = t.JointTarget;
            tr.JointTestResult = t.JointTestResult;

            tr.CreatedBy = 1;
            tr.LastUpdatedBy = 1;

            return tr;
        }

        private bool WriteWord(string tag, string value)
        {
            //Create the item to write (if the group doesn't have it, we need to insert it)
            Item[] itemToAdd = new Item[1];
            itemToAdd[0] = new Item();
            itemToAdd[0].ItemName = tag;

            //create the item that contains the value to write
            ItemValue[] writeValues = new ItemValue[1];
            writeValues[0] = new ItemValue(itemToAdd[0]);

            //make a scan of group to see if it already contains the item
            bool itemFound = false;
            foreach (Item item in _view.groupWrite.Items)
            {
                if (item.ItemName == itemToAdd[0].ItemName)
                {
                    // if it find the item i set the new value
                    writeValues[0].ServerHandle = item.ServerHandle;
                    itemFound = true;
                }
            }

            if (!itemFound)
            {
                //if it doesn't find it, we add it
                _view.groupWrite.AddItems(itemToAdd);
                writeValues[0].ServerHandle = _view.groupWrite.Items[_view.groupWrite.Items.Length - 1].ServerHandle;
            }
            //set the value to write
            writeValues[0].Value = value;
            //write

            try
            {
                _view.groupWrite.Write(writeValues);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool WriteString(string tag, string value)
        {
            //Create the item to write (if the group doesn't have it, we need to insert it)
            Item[] itemToAdd = new Item[1];
            itemToAdd[0] = new Item();
            itemToAdd[0].ItemName = tag;

            //create the item that contains the value to write
            ItemValue[] writeValues = new Opc.Da.ItemValue[1];
            writeValues[0] = new ItemValue(itemToAdd[0]);

            //make a scan of group to see if it already contains the item
            bool itemFound = false;
            foreach (Item item in _view.groupWrite.Items)
            {
                if (item.ItemName == itemToAdd[0].ItemName)
                {
                    // if it find the item i set the new value
                    writeValues[0].ServerHandle = item.ServerHandle;
                    itemFound = true;
                }
            }

            if (!itemFound)
            {
                //if it doesn't find it, we add it
                _view.groupWrite.AddItems(itemToAdd);
                writeValues[0].ServerHandle = _view.groupWrite.Items[_view.groupWrite.Items.Length - 1].ServerHandle;
            }
            //set the value to write
            writeValues[0].Value = value;
            //write

            try
            {
                _view.groupWrite.Write(writeValues);
                return true;
            }
            catch
            {
                return false;
            }

        }

        private void MakeReady(object sender, EventArgs e)
        {
            if (_view.systemReady)
            {
                if (WriteWordInterLock(_view.tagTraceabilityReady, 0))
                    _view.systemReady = false;
            }
            else
            {
                if (WriteWordInterLock(_view.tagTraceabilityReady, 1))
                    _view.systemReady = true;
            }
        }

        private void InterLock(object sender, EventArgs e)
        {
            bool result = false;

            Task t = Task.Run(() =>
            {
                result = WriteWordInterLock(_view.tagClockReady, 1);
            });
            TimeSpan ts = TimeSpan.FromMilliseconds(2000);
            //if (!t.Wait(ts))
            //    Console.WriteLine("The timeout interval elapsed.");

            if (!t.Wait(ts))
            {
                _view.connectedPlc = false;
                _view.systemReady = false;
                Disconnect();
                _view.ResultnMessage = "Inter Lock failed!";
                //Application.Exit();
            }
        }

        private bool WriteWordInterLock(string tag, int value)
        {
            //Create the item to write (if the group doesn't have it, we need to insert it)
            Item[] itemToAdd = new Item[1];
            itemToAdd[0] = new Item();
            itemToAdd[0].ItemName = tag;

            //create the item that contains the value to write
            ItemValue[] writeValues = new ItemValue[1];
            writeValues[0] = new ItemValue(itemToAdd[0]);

            //make a scan of group to see if it already contains the item
            bool itemFound = false;
            foreach (Item item in _view.groupWrite.Items)
            {
                if (item.ItemName == itemToAdd[0].ItemName)
                {
                    // if it find the item i set the new value
                    writeValues[0].ServerHandle = item.ServerHandle;
                    itemFound = true;
                }
            }

            if (!itemFound)
            {
                //if it doesn't find it, we add it
                _view.groupWrite.AddItems(itemToAdd);
                writeValues[0].ServerHandle = _view.groupWrite.Items[_view.groupWrite.Items.Length - 1].ServerHandle;
            }
            //set the value to write
            writeValues[0].Value = value;
            //write

            try
            {
                _view.groupWrite.Write(writeValues);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Disconnect(object sender, EventArgs e)
        {
            _view.connectedPlc = !Disconnect();
        }

        private void Connect(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_view.serverUrl))
            {
                _view.connectedPlc = Connect(_view.serverUrl);

                if (_view.connectedPlc)
                    LoadCurrentValue(_view.groupRead);
            }
        }

        private bool Connect(string serverUrl)
        {
            /* When the factory creates an HDA server, it passes along 2 parameters:
             *    SerializationInfo info
             *    StreamingContext context
             *
             * The Factory class casts the COM object (pointing to the HDA server) to the IServer interface.
             * All calls to the interface or proxied to the COM object.
             */


            Opc.URL url = new Opc.URL(serverUrl);
            OpcCom.Factory fact = new OpcCom.Factory();
            _view.daServer = new Server(fact, url);

            try
            {
                //_hdaServer.Connect();
                try
                {
                    _view.daServer.Connect(url, new Opc.ConnectData(new System.Net.NetworkCredential()));
                }
                catch (Exception ex)
                {
                    _view.ResultnMessage = ex.Message;
                    return false;
                }
                //Console.WriteLine(String.Format("Connect to server {0}", serverName));

                //3rd Create a group if items  
                #region Group read interlock and Machine status
                _view.groupStateRead = new SubscriptionState();
                _view.groupStateRead.Name = "InterLockGroup";
                _view.groupStateRead.UpdateRate = 1000;// this isthe time between every reads from OPC server
                _view.groupStateRead.Active = true;//this must be true if you the group has to read value
                _view.groupRead = (Subscription)_view.daServer.CreateSubscription(_view.groupStateRead);
                _view.groupRead.DataChanged += new DataChangedEventHandler(_view.group_DataChanged);//callback when the data are readed                            

                // add items to the group    (in Rockwell names are identified like [Name of PLC in the server]Block of word:number of word,number of consecutive readed words)   
                if (_view.groupRead.Items != null)
                    _view.groupRead.RemoveItems(_view.groupRead.Items);

                _view.items = new Item[_view.plcTags.Count];
                int i = 0;
                foreach (var tag in _view.plcTags)
                {
                    _view.items[i] = new Item();
                    _view.items[i].ItemName = _view.tagMainBlock + tag.PlcTag;
                    _view.items[i].Active = true;

                    i++;
                }
                _view.items = _view.groupRead.AddItems(_view.items);
                #endregion

                #region Group write
                _view.groupStateWrite = new SubscriptionState();
                _view.groupStateWrite.Name = "WriteInterLock";
                _view.groupStateWrite.Active = false;//not needed to read if you want to write only
                _view.groupWrite = (Subscription)_view.daServer.CreateSubscription(_view.groupStateWrite);
                #endregion

                return true;
            }
            catch (Opc.ConnectFailedException opcConnExc)
            {
                MessageBox.Show(String.Format("Could not connect to server {0}{1}{2}"
                                            , serverUrl
                                            , Environment.NewLine
                                            , opcConnExc.ToString())
                                            , "Connection failed!"
                                            , MessageBoxButtons.OK
                                            , MessageBoxIcon.Error);
                return false;
            }
        }

        private bool Disconnect()
        {
            if (_view.daServer != null && _view.daServer.IsConnected)
                _view.daServer.Disconnect();

            return true;
        }

        private void Initailization(object sender, EventArgs e)
        {
            int machineId = ConvertToInt(ConfigurationManager.AppSettings["MachineId"].ToString());
            _view.serverUrl = ConfigurationManager.AppSettings["DefaultUrl"].ToString();
            _view.tagMainBlock = ConfigurationManager.AppSettings["MainBlock"].ToString();

            var m = _serviceMachine.GetByID(machineId);
            if (m != null)
            {
                _view.machine = m;

                var result = _servicePLCTag.GetAll();
                _view.plcTags = result.Where(x => x.MachineId == machineId || x.MachineId == 99).ToList();

                if (_view.plcTags.Count > 0)
                {
                    string clockTag = _view.plcTags.Where(x => x.TypeCode == "INTER_LOCK").FirstOrDefault().PlcTag;
                    string readyTag = _view.plcTags.Where(x => x.TypeCode == "SYSTEM_READY").FirstOrDefault().PlcTag;
                    _view.tagClockReady = _view.tagMainBlock + clockTag;
                    _view.tagTraceabilityReady = _view.tagMainBlock + readyTag;
                }
            }
        }

        private void LoadCurrentValue(Subscription groupRead)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                var currentResult = _view.groupRead.Read(_view.groupRead.Items).ToList();

                //Machine
                var mac5 = _view.machine;
                var onlineTag5 = _view.tagMainBlock + "ST4StatusMc";
                mac5.OnlineFlag = Convert.ToInt32(currentResult.Where(x => x.ItemName == onlineTag5).FirstOrDefault().Value);

                var reqLoggingTag5 = _view.tagMainBlock + "ST4ReqLogging";
                mac5.RequestLogging = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqLoggingTag5).FirstOrDefault().Value);

                var completeLoggingTag5 = _view.tagMainBlock + "ST4LoggingApp";
                mac5.CompletedLogging = Convert.ToInt32(currentResult.Where(x => x.ItemName == completeLoggingTag5).FirstOrDefault().Value);

                var reqVerifyTag5 = _view.tagMainBlock + "ST4ReqChkCodeVerify";
                mac5.RequestVerifyCode = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqVerifyTag5).FirstOrDefault().Value);

                var verifyResultTag5 = _view.tagMainBlock + "ST4CodeVerifyResult";
                mac5.CodeVerifyResult = Convert.ToInt32(currentResult.Where(x => x.ItemName == verifyResultTag5).FirstOrDefault().Value);

                int receivedData5;
                var positionTag5 = _view.tagMainBlock + "ST4TestResult[19]";
                var succes5 = Int32.TryParse(currentResult.Where(x => x.ItemName == positionTag5).FirstOrDefault().Value.ToString(), out receivedData5);
                if (!succes5)
                    receivedData5 = 0;
                mac5.TighteningPosition = receivedData5;

                _view.machine = mac5;
                _view.ResultnMessage = string.Empty;
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                _view.ResultnMessage = ex.Message;
                Cursor.Current = Cursors.Default;
                return;
            }
        }


        private bool InvalidDataTag(string value, string tagName, out string tmpMsg)
        {
            bool result = false;
            string returnMsg = string.Empty;
            var tags = _view.plcTags;
            var tag = tags.Where(x => _view.tagMainBlock + x.PlcTag == tagName).FirstOrDefault();

            if (tag != null)
            {
                if (tag.RequiredFlag)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        returnMsg = tag.PlcTag + " is required value.";
                        result = true;
                    }
                }

                if (tag.DataType.ToUpper() == "STRING")
                {
                    int len1 = value.Length;
                    int len2 = tag.Length;
                    if (len2 > 0)
                    {
                        if (InvalidString(len1, len2))
                        {
                            returnMsg = tag.PlcTag + " tag must be " + len2 + " digits.";
                            result = true;
                        }
                    }
                }

                if (tag.DataType.ToUpper() == "DECIMAL")
                {
                    if (string.IsNullOrEmpty(value))
                        value = "0";

                    if (InvalidDecimal(value))
                    {
                        returnMsg = tag.PlcTag + " tag must be Decimal type.";
                        result = true;
                    }
                }

                if (tag.DataType.ToUpper() == "INT")
                {
                    if (string.IsNullOrEmpty(value))
                        value = "0";

                    if (InvalidInt(value))
                    {
                        returnMsg = tag.PlcTag + " tag must be Integer type.";
                        result = true;
                    }
                }

                if (tag.DataType.ToUpper() == "BOOL")
                {
                    if (string.IsNullOrEmpty(value))
                        value = "0";

                    if (InvalidBoolean(value))
                    {
                        returnMsg = tag.PlcTag + " tag must be Boolean type.";
                        result = true;
                    }
                }
            }

            tmpMsg = returnMsg;
            return result;
        }

        public string RemoveSpecialCharacters(string str)
        {
            string spath = Regex.Replace(str, "[^a-zA-Z0-9_.:]+", "\\", RegexOptions.Compiled);
            spath = spath.Replace("\\.", ".");
            spath = spath.Replace("\\D", "D");
            spath = spath.Replace("\\_", "_");
            return spath.Replace("\\E", "E");
        }

        private decimal ConvertToDecimal(string number)
        {
            decimal myDecimal;
            if (string.IsNullOrEmpty(number))
                number = "0";

            bool result = decimal.TryParse(number, out myDecimal);

            if (result)
                return myDecimal;
            else
                return 0;
        }

        private int ConvertToInt(string number)
        {
            int myInt;
            if (string.IsNullOrEmpty(number))
                number = "0";

            bool result = int.TryParse(number, out myInt);

            if (result)
                return myInt;
            else
                return 0;
        }

        private bool InvalidString(int len1, int len2)
        {
            return len1 != len2;
        }

        private bool InvalidDecimal(string number)
        {
            decimal myDecimal;
            return !decimal.TryParse(number, out myDecimal);
        }

        private bool InvalidInt(string number)
        {
            int myInt;
            return !int.TryParse(number, out myInt);
        }

        private bool InvalidBoolean(string number)
        {
            return !(number == "0" || number == "1");
        }

        public bool WriteLog(string strFileName, string strMessage)
        {
            try
            {
                FileStream objFilestream = new FileStream(string.Format("{0}\\{1}", Path.GetTempPath(), strFileName), FileMode.Append, FileAccess.Write);
                StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
                objStreamWriter.WriteLine(strMessage);
                objStreamWriter.Close();
                objFilestream.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
