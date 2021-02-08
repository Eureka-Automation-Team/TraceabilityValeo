using Opc.Da;
using OPCUserInterface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trace.Data;
using Trace.Data.Service;
using Trace.Domain.Models;
using Trace.Domain.Services;

namespace Trace.OpcHandlerMachine01.Presenters
{

    public class MainPresenter
    {
        //OPCClient OPC = new OPCClient();
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

            var tagName = _view.tagMainBlock + "ST1CodeVerify";
            var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("Get PLC Tag value time : {0}"
                                                                 , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

            var loggings = _serviceTraceLog.GetListByItemCode(value.ToString());
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("Get Item Code : {0} from database time : {1}"
                                                                , value.ToString()
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

            if (loggings.Where(x => x.MachineId == 1).Count() == 0)
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
            bool reactResult = false;

            //reactResult = WriteWord(_view.tagMainBlock + "ST1CodeVerifyResult", _machine.CodeVerifyResult.ToString());

            /*---- Start Code Migration ----*/
            reactResult = _view.OPC.WriteVar("CodeVerifyResultWrite", Convert.ToSByte(_machine.CodeVerifyResult));
            /*---- End Code Migration ----*/

            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("Write PLC Tag : {0}  Value = [{2}] => Complete Time : {1}"
                                                                , "ST1CodeVerifyResult"
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
                    //var reactResult = WriteWord(_view.tagMainBlock + "ST1LoggingApp", machineTmp.CompletedLogging.ToString());

                    /*---- Start Code Migration ----*/
                    var reactResult = _view.OPC.WriteVar("LoggingAppWrite", Convert.ToSByte(machineTmp.CompletedLogging));
                    /*---- End Code Migration ----*/

                    WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Write PLC Tag : {0}  Value = [{2}] => Complete Time : {1}"
                                                                , "ST1LoggingApp"
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

            var tagsCamera = (from tag in machineTags
                              where tag.TypeCode == "DATA_CAMERA_RESULT"
                              select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

            var tagsDisable = (from tag in machineTags
                               where tag.EnableFlag == false
                               select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

            //Default Station and Machine
            trace.StationId = machine.StationId;
            trace.MachineId = machine.Id;
            trace.ProductionDate = DateTime.Now;

            #region Validate and Default existing log by Item Code.
            var tagName = _view.tagMainBlock + "ST1Code";
            var itemCode = r.Where(x => x.ItemName == tagName).FirstOrDefault().Value.ToString();
            var loggings = _serviceTraceLog.GetListByItemCode(itemCode)
                                            .Where(x => x.MachineId == machine.Id).ToList();

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

            #region Keep Master Logging
            foreach (var item in r.Where(x => !tagsDisable.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
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
                    if (item.ItemName == _view.tagMainBlock + "ST1Code")
                        trace.ItemCode = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST1Final_Judgment")
                    {
                        if (Convert.ToInt32(item.Value) != 0)
                        {
                            trace.FinalResult = Convert.ToInt32(item.Value);
                        }
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST1ModelRunning")
                        trace.ModelRunning = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST1RepairTime")
                        trace.RepairTime = Convert.ToInt32(item.Value);

                    if (item.ItemName == _view.tagMainBlock + "ST1ModelRunning2")
                        trace.ModelRunningFlag = Convert.ToInt32(item.Value);
                }
            }
            #endregion


            #region Keep Part Assemblies
            trace.PartAssemblies = new List<PartAssemblyModel>();
            string partActuaterSN = string.Empty;
            foreach (var item in r.Where(x => tagsPart.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                //tmpMsg = string.Empty;
                //if (InvalidDataTag((item.Value == null) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                //{
                //    invalid = true;
                //    if (!string.IsNullOrEmpty(errMsg))
                //        errMsg += Environment.NewLine;

                //    errMsg += tmpMsg;
                //}
                if (!invalid)
                {
                    PartAssemblyModel part = new PartAssemblyModel();
                    if (trace.ModelRunningFlag == 1)
                    {
                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[0]")
                        {
                            part.LineNumber = 1;
                            part.PartName = "UPR Actuator P/N";
                            part.SerialNumber = item.Value.ToString();
                            if (!string.IsNullOrEmpty(part.SerialNumber))
                                partActuaterSN = part.SerialNumber.Substring(11, (part.SerialNumber.Length - 11));
                        }
                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[1]")
                        {
                            part.LineNumber = 2;
                            part.PartName = "UPR Actuator S/N";
                            part.SerialNumber = partActuaterSN;
                        }

                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[3]")
                        {
                            part.LineNumber = 4;
                            part.PartName = "Vanes";
                            part.SerialNumber = item.Value.ToString();
                        }

                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[4]")
                        {
                            part.LineNumber = 5;
                            part.PartName = "Vane pack RH";
                            part.SerialNumber = item.Value.ToString();
                        }
                        /*if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[16]")
                        {
                            part.LineNumber = 6;
                            part.PartName = "Blind vane set";
                            part.SerialNumber = item.Value.ToString();
                        }*/
                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[5]")
                        {
                            part.LineNumber = 7;
                            part.PartName = "Pull rod";
                            part.SerialNumber = item.Value.ToString();
                        }
                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[6]")
                        {
                            part.LineNumber = 8;
                            part.PartName = "Lever";
                            part.SerialNumber = item.Value.ToString();
                        }
                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[7]")
                        {
                            part.LineNumber = 9;
                            part.PartName = "Spacer";
                            part.SerialNumber = item.Value.ToString();
                        }
                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[8]")
                        {
                            part.LineNumber = 10;
                            part.PartName = "Tether";
                            part.SerialNumber = item.Value.ToString();
                        }
                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[9]")
                        {
                            part.LineNumber = 11;
                            part.PartName = "UPR Actuator BOX S/N ";
                            part.SerialNumber = item.Value.ToString();
                        }
                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[10]")
                        {
                            part.LineNumber = 12;
                            part.PartName = "UPR Van set LH BOX S/N";
                            part.SerialNumber = item.Value.ToString();
                        }
                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[11]")
                        {
                            part.LineNumber = 13;
                            part.PartName = "UPR Van set RH BOX S/N";
                            part.SerialNumber = item.Value.ToString();
                        }
                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[12]")
                        {
                            part.LineNumber = 14;
                            part.PartName = "Tether BOX S/N";
                            part.SerialNumber = item.Value.ToString();
                        }
                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[13]")
                        {
                            part.LineNumber = 15;
                            part.PartName = "UPR Frame BOX S/N";
                            part.SerialNumber = item.Value.ToString();
                        }
                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[14]")
                        {
                            part.LineNumber = 16;
                            part.PartName = "PullRod BOX S/N";
                            part.SerialNumber = item.Value.ToString();
                        }
                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[15]")
                        {
                            part.LineNumber = 17;
                            part.PartName = "Spacer BOX S/N";
                            part.SerialNumber = item.Value.ToString();
                        }
                        if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[17]")
                        {
                            part.LineNumber = 18;
                            part.PartName = "Head screw M5X20";
                            part.SerialNumber = item.Value.ToString();
                        }
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[2]")
                    {
                        part.LineNumber = 3;
                        part.PartName = "UPR Frame";
                        part.SerialNumber = item.Value.ToString();
                    }



                    if (part.PartName != null)
                        trace.PartAssemblies.Add(part);
                }
            }
            #endregion

            List<TighteningRepairModel> tRepairs = new List<TighteningRepairModel>();
            TighteningRepairModel tRepair = new TighteningRepairModel();
            TighteningResultModel tmp = new TighteningResultModel();
            if (trace.ModelRunningFlag == 1)
            {
                #region Keep Tightening

                if (trace.TighteningResults == null)
                    trace.TighteningResults = new List<TighteningResultModel>();

                foreach (var item in r.Where(x => tagsTightening.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
                {
                    if (!invalid)
                    {
                        //No.1                  
                        if (item.ItemName == _view.tagMainBlock + "ST1TestResult[0]")
                        {
                            TighteningResultModel t = new TighteningResultModel();
                            t.No = "1";
                            t.Result = ConvertToDecimal(item.Value.ToString());
                            t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[0]").FirstOrDefault().Value.ToString());
                            t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[0]").FirstOrDefault().Value.ToString());
                            t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[0]").FirstOrDefault().Value.ToString());
                            t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[0]").FirstOrDefault().Value.ToString();

                            //JointsControlAngle Result 
                            //t.JointResult = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestResult[10]").FirstOrDefault().Value);
                            //t.JointMin = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[10]").FirstOrDefault().Value);
                            //t.JointMax = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[10]").FirstOrDefault().Value);
                            //t.JointTarget = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[10]").FirstOrDefault().Value);
                            //t.JointTestResult = t.TestResult;// r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[10]").FirstOrDefault().Value.ToString();

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
                        if (item.ItemName == _view.tagMainBlock + "ST1TestResult[1]")
                        {
                            TighteningResultModel t = new TighteningResultModel();
                            t.No = "2";
                            t.Result = ConvertToDecimal(item.Value.ToString());
                            t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[1]").FirstOrDefault().Value.ToString());
                            t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[1]").FirstOrDefault().Value.ToString());
                            t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[1]").FirstOrDefault().Value.ToString());
                            t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[1]").FirstOrDefault().Value.ToString();

                            //JointsControlAngle Result 
                            //t.JointResult = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestResult[11]").FirstOrDefault().Value);
                            //t.JointMin = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[11]").FirstOrDefault().Value);
                            //t.JointMax = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[11]").FirstOrDefault().Value);
                            //t.JointTarget = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[11]").FirstOrDefault().Value);
                            //t.JointTestResult = t.TestResult;// r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[11]").FirstOrDefault().Value.ToString();

                            tRepair = autoMappingRepair(t);
                            List<TighteningResultModel> tExist = new List<TighteningResultModel>();
                            tExist = trace.TighteningResults.Where(x => x.No == t.No).ToList();

                            if (tExist.Count() == 0)
                            {
                                trace.TighteningResults.Add(t);

                                if (!string.IsNullOrEmpty(t.TestResult))
                                    tRepairs.Add(tRepair);
                            }
                            else if (tExist.Where(x => (string.IsNullOrEmpty(x.TestResult) ? "NOK" : x.TestResult) == "NOK").Count() > 0)
                            {
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
                                        //c.RepairFlag = true;
                                        return c;
                                    }).ToList();

                                if (!string.IsNullOrEmpty(t.TestResult))
                                    tRepairs.Add(tRepair);
                            }
                        }

                        //No.3
                        if (item.ItemName == _view.tagMainBlock + "ST1TestResult[2]")
                        {
                            TighteningResultModel t = new TighteningResultModel();
                            t.No = "3";
                            t.Result = ConvertToDecimal(item.Value.ToString());
                            t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[2]").FirstOrDefault().Value.ToString());
                            t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[2]").FirstOrDefault().Value.ToString());
                            t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[2]").FirstOrDefault().Value.ToString());
                            t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[2]").FirstOrDefault().Value.ToString();

                            //JointsControlAngle Result 
                            //t.JointResult = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestResult[12]").FirstOrDefault().Value);
                            //t.JointMin = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[12]").FirstOrDefault().Value);
                            //t.JointMax = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[12]").FirstOrDefault().Value);
                            //t.JointTarget = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[12]").FirstOrDefault().Value);
                            //t.JointTestResult = t.TestResult;// r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[12]").FirstOrDefault().Value.ToString();

                            tRepair = autoMappingRepair(t);
                            List<TighteningResultModel> tExist = new List<TighteningResultModel>();

                            tExist = trace.TighteningResults.Where(x => x.No == t.No).ToList();

                            if (tExist.Count() == 0)
                            {
                                trace.TighteningResults.Add(t);

                                if (!string.IsNullOrEmpty(t.TestResult))
                                    tRepairs.Add(tRepair);
                            }
                            else if (tExist.Where(x => (string.IsNullOrEmpty(x.TestResult) ? "NOK" : x.TestResult) == "NOK").Count() > 0)
                            {
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
                                        //c.RepairFlag = true;
                                        return c;
                                    }).ToList();

                                if (!string.IsNullOrEmpty(t.TestResult))
                                    tRepairs.Add(tRepair);
                            }
                        }

                        //No.4
                        if (item.ItemName == _view.tagMainBlock + "ST1TestResult[3]")
                        {
                            TighteningResultModel t = new TighteningResultModel();
                            t.No = "4";
                            t.Result = ConvertToDecimal(item.Value.ToString());
                            t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[3]").FirstOrDefault().Value.ToString());
                            t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[3]").FirstOrDefault().Value.ToString());
                            t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[3]").FirstOrDefault().Value.ToString());
                            t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[3]").FirstOrDefault().Value.ToString();

                            //JointsControlAngle Result 
                            //t.JointResult = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestResult[13]").FirstOrDefault().Value);
                            //t.JointMin = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[13]").FirstOrDefault().Value);
                            //t.JointMax = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[13]").FirstOrDefault().Value);
                            //t.JointTarget = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[13]").FirstOrDefault().Value);
                            //t.JointTestResult = t.TestResult;// r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[13]").FirstOrDefault().Value.ToString();

                            tRepair = autoMappingRepair(t);
                            List<TighteningResultModel> tExist = new List<TighteningResultModel>();
                            tExist = trace.TighteningResults.Where(x => x.No == t.No).ToList();

                            if (tExist.Count() == 0)
                            {
                                trace.TighteningResults.Add(t);

                                if (!string.IsNullOrEmpty(t.TestResult))
                                    tRepairs.Add(tRepair);
                            }
                            else if (tExist.Where(x => (string.IsNullOrEmpty(x.TestResult) ? "NOK" : x.TestResult) == "NOK").Count() > 0)
                            {
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
                                        //c.RepairFlag = true;
                                        return c;
                                    }).ToList();

                                if (!string.IsNullOrEmpty(t.TestResult))
                                    tRepairs.Add(tRepair);
                            }
                        }

                        //No.5
                        if (item.ItemName == _view.tagMainBlock + "ST1TestResult[4]")
                        {
                            TighteningResultModel t = new TighteningResultModel();
                            t.No = "5";
                            t.Result = ConvertToDecimal(item.Value.ToString());
                            t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[4]").FirstOrDefault().Value.ToString());
                            t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[4]").FirstOrDefault().Value.ToString());
                            t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[4]").FirstOrDefault().Value.ToString());
                            t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[4]").FirstOrDefault().Value.ToString();

                            //JointsControlAngle Result 
                            //t.JointResult = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestResult[14]").FirstOrDefault().Value);
                            //t.JointMin = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[14]").FirstOrDefault().Value);
                            //t.JointMax = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[14]").FirstOrDefault().Value);
                            //t.JointTarget = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[14]").FirstOrDefault().Value);
                            //t.JointTestResult = t.TestResult;// r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[14]").FirstOrDefault().Value.ToString();

                            tRepair = autoMappingRepair(t);
                            List<TighteningResultModel> tExist = new List<TighteningResultModel>();
                            tExist = trace.TighteningResults.Where(x => x.No == t.No).ToList();

                            if (tExist.Count() == 0)
                            {
                                trace.TighteningResults.Add(t);

                                if (!string.IsNullOrEmpty(t.TestResult))
                                    tRepairs.Add(tRepair);
                            }
                            else if (tExist.Where(x => (string.IsNullOrEmpty(x.TestResult) ? "NOK" : x.TestResult) == "NOK").Count() > 0)
                            {
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
                                        //c.RepairFlag = true;
                                        return c;
                                    }).ToList();

                                if (!string.IsNullOrEmpty(t.TestResult))
                                    tRepairs.Add(tRepair);
                            }
                        }

                        //No.6
                        if (item.ItemName == _view.tagMainBlock + "ST1TestResult[5]")
                        {
                            TighteningResultModel t = new TighteningResultModel();
                            t.No = "6";
                            t.Result = ConvertToDecimal(item.Value.ToString());
                            t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[5]").FirstOrDefault().Value.ToString());
                            t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[5]").FirstOrDefault().Value.ToString());
                            t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[5]").FirstOrDefault().Value.ToString());
                            t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[5]").FirstOrDefault().Value.ToString();

                            //JointsControlAngle Result 
                            //t.JointResult = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestResult[15]").FirstOrDefault().Value);
                            //t.JointMin = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[15]").FirstOrDefault().Value);
                            //t.JointMax = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[15]").FirstOrDefault().Value);
                            //t.JointTarget = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[15]").FirstOrDefault().Value);
                            //t.JointTestResult = t.TestResult;// r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[15]").FirstOrDefault().Value.ToString();

                            tRepair = autoMappingRepair(t);
                            List<TighteningResultModel> tExist = new List<TighteningResultModel>();
                            tExist = trace.TighteningResults.Where(x => x.No == t.No).ToList();

                            if (tExist.Count() == 0)
                            {
                                trace.TighteningResults.Add(t);

                                if (!string.IsNullOrEmpty(t.TestResult))
                                    tRepairs.Add(tRepair);
                            }
                            else if (tExist.Where(x => (string.IsNullOrEmpty(x.TestResult) ? "NOK" : x.TestResult) == "NOK").Count() > 0)
                            {
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
                                        //c.RepairFlag = true;
                                        return c;
                                    }).ToList();

                                if (!string.IsNullOrEmpty(t.TestResult))
                                    tRepairs.Add(tRepair);
                            }
                        }

                        //No.7
                        if (item.ItemName == _view.tagMainBlock + "ST1TestResult[6]")
                        {
                            TighteningResultModel t = new TighteningResultModel();
                            t.No = "7";
                            t.Result = ConvertToDecimal(item.Value.ToString());
                            t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[6]").FirstOrDefault().Value.ToString());
                            t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[6]").FirstOrDefault().Value.ToString());
                            t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[6]").FirstOrDefault().Value.ToString());
                            t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[6]").FirstOrDefault().Value.ToString();

                            //JointsControlAngle Result 
                            //t.JointResult = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestResult[16]").FirstOrDefault().Value);
                            //t.JointMin = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[16]").FirstOrDefault().Value);
                            //t.JointMax = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[16]").FirstOrDefault().Value);
                            //t.JointTarget = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[16]").FirstOrDefault().Value);
                            //t.JointTestResult = t.TestResult;// r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[16]").FirstOrDefault().Value.ToString();

                            tRepair = autoMappingRepair(t);
                            List<TighteningResultModel> tExist = new List<TighteningResultModel>();
                            tExist = trace.TighteningResults.Where(x => x.No == t.No).ToList();

                            if (tExist.Count() == 0)
                            {
                                trace.TighteningResults.Add(t);

                                if (!string.IsNullOrEmpty(t.TestResult))
                                    tRepairs.Add(tRepair);
                            }
                            else if (tExist.Where(x => (string.IsNullOrEmpty(x.TestResult) ? "NOK" : x.TestResult) == "NOK").Count() > 0)
                            {
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
                                        //c.RepairFlag = true;
                                        return c;
                                    }).ToList();

                                if (!string.IsNullOrEmpty(t.TestResult))
                                    tRepairs.Add(tRepair);
                            }
                        }

                        //No.8
                        if (item.ItemName == _view.tagMainBlock + "ST1TestResult[7]")
                        {
                            TighteningResultModel t = new TighteningResultModel();
                            t.No = "8";
                            t.Result = ConvertToDecimal(item.Value.ToString());
                            t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[7]").FirstOrDefault().Value.ToString());
                            t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[7]").FirstOrDefault().Value.ToString());
                            t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[7]").FirstOrDefault().Value.ToString());
                            t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[7]").FirstOrDefault().Value.ToString();

                            //JointsControlAngle Result
                            //t.JointResult = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestResult[17]").FirstOrDefault().Value);
                            //t.JointMin = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[17]").FirstOrDefault().Value);
                            //t.JointMax = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[17]").FirstOrDefault().Value);
                            //t.JointTarget = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[17]").FirstOrDefault().Value);
                            //t.JointTestResult = t.TestResult;// r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[17]").FirstOrDefault().Value.ToString();

                            tRepair = autoMappingRepair(t);
                            List<TighteningResultModel> tExist = new List<TighteningResultModel>();
                            tExist = trace.TighteningResults.Where(x => x.No == t.No).ToList();

                            if (tExist.Count() == 0)
                            {
                                trace.TighteningResults.Add(t);

                                if (!string.IsNullOrEmpty(t.TestResult))
                                    tRepairs.Add(tRepair);
                            }
                            else if (tExist.Where(x => (string.IsNullOrEmpty(x.TestResult) ? "NOK" : x.TestResult) == "NOK").Count() > 0)
                            {
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
                                        //c.RepairFlag = true;
                                        return c;
                                    }).ToList();

                                if (!string.IsNullOrEmpty(t.TestResult))
                                    tRepairs.Add(tRepair);
                            }
                        }
                    }
                }
                #endregion

                #region Keep Camera Result
                trace.CameraResults = new List<CameraResultModel>();

                foreach (var item in r.Where(x => tagsCamera.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
                {
                    CameraResultModel cam = new CameraResultModel();
                    if (item.ItemName == _view.tagMainBlock + "ST1TestResult[8]")
                    {
                        cam.CameraName = "Lever Assy LH";
                        cam.TestResult = item.Value.ToString();
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST1TestResult[9]")
                    {
                        cam.CameraName = "Lever Assy RH";
                        cam.TestResult = item.Value.ToString();
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST1TestResult[17]")
                    {
                        cam.CameraName = "Check Spacer";
                        cam.TestResult = item.Value.ToString();
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST1TestResult[18]")
                    {
                        cam.CameraName = "Check Vane RH";
                        cam.TestResult = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1TestResult[19]")
                    {
                        cam.CameraName = "Check Vane LH";
                        cam.TestResult = item.Value.ToString();
                    }

                    trace.CameraResults.Add(cam);
                }
                #endregion
            }

            #region Insert/Update Logging Detail
            if (invalid)
            {
                WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Data Invalid Item Code : {0}, at {1} Message : {2}"
                                                                , trace.ItemCode.ToString()
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
                                                                , errMsg));
                var mac = _view.machine;
                mac.MessageResult = errMsg;
                _view.ResultnMessage = mac.MessageResult;
                mac.CompletedLogging = 3;
                _view.machine = mac;
                result = false;
            }
            else
            {
                TraceabilityLogModel logResult = new TraceabilityLogModel();
                if (trace.FinalResult != 0)
                    trace.FinishFlag = true;

                if (trace.Id == 0 && !string.IsNullOrEmpty(trace.ItemCode))
                {
                    WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Insert New Item Code : {0}, at {1}"
                                                                , trace.ItemCode.ToString()
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
                    logResult = _serviceTraceLog.Create(trace);
                    foreach (var item in logResult.TighteningResults.OrderBy(o => o.No))
                    {
                        var cRepair = tRepairs.Where(x => x.No == item.No).FirstOrDefault();
                        if (cRepair != null)
                        {
                            cRepair.TighteningResultId = item.Id;
                            _serviceTigtheningRepair.Create(cRepair);
                        }
                    }

                    result = true;
                }
                else if (trace.Id != 0 && !string.IsNullOrEmpty(trace.ItemCode))
                {
                    WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Update Item Code : {0}, at {1}"
                                                                , trace.ItemCode.ToString()
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
                    logResult = _serviceTraceLog.Update(trace);
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
                                cRepair.TighteningResultId = item.Id;
                                _serviceTigtheningRepair.Create(cRepair);
                            }
                        }
                    }

                    result = true;
                }

                if (trace.FinishFlag)
                {
                    CameraResultModel model = new CameraResultModel();
                    model.TraceLogId = logResult.Id;
                    var cams = _serviceCameraResult.GetByPrimary(model);
                    foreach (var item in trace.CameraResults)
                    {
                        var cam = cams.Where(x => x.CameraName == item.CameraName)
                                .Select(c => {
                                    c.TestResult = item.TestResult;
                                    return c;
                                }).FirstOrDefault();

                        _serviceCameraResult.Update(cam);
                    }
                }
            }
            #endregion

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
                Opc.IRequest req;
                _view.groupWrite.Write(writeValues, 10, new WriteCompleteEventHandler(WriteCompleteCallback), out req);
                _view.groupRead.Read(itemToAdd, 5, new ReadCompleteEventHandler(ReadCompleteCallback), out req);
                _view.groupWrite.RemoveItems(writeValues);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ReadCompleteCallback(object requestHandle, ItemValueResult[] results)
        {
            foreach (Opc.Da.ItemValueResult readResult in results)
            {
                WriteLog("WriteWord" + _view.machine.Id + ".txt", String.Format("Read Value = {1} || Quality : {2}", readResult.Value, readResult.Quality));
                //, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
            }
            Console.WriteLine();
        }

        private void WriteCompleteCallback(object requestHandle, Opc.IdentifiedResult[] results)
        {
            foreach (Opc.IdentifiedResult writeResult in results)
            {
                WriteLog("WriteWord" + _view.machine.Id + ".txt", String.Format("Item Name : {0} || Succeeded = {1} || Time : {2}", writeResult.ItemName, writeResult.ResultID.Succeeded().ToString()
                                                                 , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
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
                _view.OPC.WriteVar("TraceabilityRdyWrite", false);
            }
            else
            {
                _view.OPC.WriteVar("TraceabilityRdyWrite", true);
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
                // _view.groupStateRead.UpdateRate = 1000;// this is the time between every reads from OPC server
                _view.groupStateRead.Active = false;//this must be true if you the group has to read value
                _view.groupRead = (Subscription)_view.daServer.CreateSubscription(_view.groupStateRead);
                //_view.groupRead.DataChanged += new DataChangedEventHandler(_view.group_DataChanged);//callback when the data are readed                            

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
                //_view.groupStateWrite = new SubscriptionState();
                //_view.groupStateWrite.Name = "WriteInterLock";
                //_view.groupStateWrite.Active = false;//not needed to read if you want to write only
                //_view.groupWrite = (Subscription)_view.daServer.CreateSubscription(_view.groupStateWrite);
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

            /*---- Start Code Migration ----*/
            // List of variables to monitor for events:
            _view.OPCEventVars = new List<OPCVar>()
            {
            new OPCVar("RequestVerify", "ST1ReqChkCodeVerify", OPCVarType.BOOL),
            new OPCVar("MachineStatus","ST1StatusMc", OPCVarType.SINT),
            new OPCVar("RequestLogging", "ST1ReqLogging", OPCVarType.BOOL),
            new OPCVar("ClockSystem", "ClockSystem", OPCVarType.BOOL),
            new OPCVar("TraceabilityRdy", "TraceabilityRdy", OPCVarType.BOOL),
            new OPCVar("LoggingApp", "ST1LoggingApp", OPCVarType.SINT),
            //new OPCVar("RequestLogging", "ST1ReqChkCodeVerify", OPCVarType.INT),
            //new OPCVar("DintVar1", "Program:MainProgram.DintVar1", OPCVarType.DINT),
            //new OPCVar("RealVar1", "Program:MainProgram.RealVar1", OPCVarType.REAL),
            };

            // List of variables to write to:
            _view.OPCWriteVars = new List<OPCVar>()
            {
            new OPCVar("TraceabilityRdyWrite", "TraceabilityRdy", OPCVarType.BOOL),
            new OPCVar("LoggingAppWrite","ST1LoggingApp", OPCVarType.SINT),
            new OPCVar("CodeVerifyResultWrite","ST1CodeVerifyResult", OPCVarType.SINT),
            //new OPCVar("BoolVar2", "ST1ReqChkCodeVerify", OPCVarType.BOOL),
            //new OPCVar("SintVar2","ST1StatusMc", OPCVarType.SINT),
            //new OPCVar("IntVar2", "ST1ReqChkCodeVerify", OPCVarType.INT),
            //new OPCVar("DintVar2", "Program:MainProgram.DintVar2", OPCVarType.DINT),
            //new OPCVar("RealVar2", "Program:MainProgram.RealVar2", OPCVarType.REAL),
            };
            /*---- End Code Migration ----*/

            Thread.Sleep(10000);
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

                    /*---- Start Code Migration ----*/
                    if (!_view.OPC.Init(_view.OPCEventVars, _view.OPCWriteVars, _view.serverUrl, _view.tagMainBlock))
                    {
                        _view.ComErrorMessage("Cannot establish communication with OPC server on startup.");
                        return;
                    }
                    /*---- End Code Migration ----*/
                }
            }
        }

        private void LoadCurrentValue(Subscription groupRead)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                var currentResult = _view.groupRead.Read(_view.groupRead.Items).ToList();

                //Machine 1
                var mac1 = _view.machine;
                var onlineTag = _view.tagMainBlock + "ST1StatusMc";
                mac1.OnlineFlag = Convert.ToInt32(currentResult.Where(x => x.ItemName == onlineTag).FirstOrDefault().Value);

                var reqLoggingTag = _view.tagMainBlock + "ST1ReqLogging";
                mac1.RequestLogging = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqLoggingTag).FirstOrDefault().Value);

                var completeLoggingTag = _view.tagMainBlock + "ST1LoggingApp";
                mac1.CompletedLogging = Convert.ToInt32(currentResult.Where(x => x.ItemName == completeLoggingTag).FirstOrDefault().Value);

                var reqVerifyTag = _view.tagMainBlock + "ST1ReqChkCodeVerify";
                mac1.RequestVerifyCode = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqVerifyTag).FirstOrDefault().Value);

                var verifyResultTag = _view.tagMainBlock + "ST1CodeVerifyResult";
                mac1.CodeVerifyResult = Convert.ToInt32(currentResult.Where(x => x.ItemName == verifyResultTag).FirstOrDefault().Value);

                int receivedData;
                var positionTag = _view.tagMainBlock + "ST1TestResult[19]";
                var succes = Int32.TryParse(currentResult.Where(x => x.ItemName == positionTag).FirstOrDefault().Value.ToString(), out receivedData);
                if (!succes)
                    receivedData = 0;
                mac1.TighteningPosition = receivedData;

                _view.machine = mac1;
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
            /*---- Code Migration ----*/
            bool myBool;
            return !Boolean.TryParse(number, out myBool);
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
