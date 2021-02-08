using Opc.Da;
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

namespace Trace.OpcHandlerMachine06.Presenters
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
            _view.VerityActuater += VerityActuater;
        }

        private void VerityActuater(object sender, EventArgs e)
        {
            MachineModel _machine = sender as MachineModel;
            var grpReadResult = _view.groupRead.Read(_view.groupRead.Items).ToList();
            WriteLog("VerifyActuaterUpper" + _view.machine.Id + ".txt", String.Format("<<=================== Start time : {0} ===================>>"
                                                                 , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

            WriteLog("VerifyActuaterUpper" + _view.machine.Id + ".txt", String.Format("RequestCodeActuater : {0} ", _machine.RequestCodeActuater));
            if (_machine.RequestCodeActuater)
            {
                var tagItemCode = _view.tagMainBlock + "ST5_1CodeVerify";

                var itemCode = grpReadResult.Where(x => x.ItemName == tagItemCode).FirstOrDefault().Value;

                WriteLog("VerifyActuaterUpper" + _view.machine.Id + ".txt", String.Format("Item Code : {0} => Time : {1}"
                                                                    , itemCode
                                                                    , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

                var result = _serviceTraceLog.GetListByItemCode(itemCode.ToString());
                var loggings = result.Where(x => x.MachineId == 1);
                string receiveActuatorCode = string.Empty;

                if (loggings.Count() == 0)
                {
                    _machine.ActuatorResult = 2;  //Data not found
                }
                else
                {
                    PartAssemblyModel model = new PartAssemblyModel();
                    model.TraceabilityLogId = loggings.FirstOrDefault().Id;
                    var partAssblies = _servicePartAssembly.GetByPrimary(model);

                    if (partAssblies.Count() > 0)
                    {
                        var actuaterResult = partAssblies.Where(x => x.PartName == "UPR Actuator P/N");

                        if (actuaterResult.Count() == 0)
                        {
                            _machine.ActuatorResult = 2;
                        }
                        else
                        {
                            _machine.ActuatorResult = 1;
                            receiveActuatorCode = actuaterResult.FirstOrDefault().SerialNumber;
                        }
                    }
                    else
                        _machine.ActuatorResult = 2;

                }

                WriteLog("VerifyActuaterUpper" + _view.machine.Id + ".txt", String.Format("Actuator Result : {0} => Time : {1}"
                                                            , _machine.ActuatorResult
                                                            , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
                
                WriteWord(_view.tagMainBlock + "ST5_1ReceiveCodeResult", _machine.ActuatorResult.ToString());

                WriteLog("VerifyActuaterUpper" + _view.machine.Id + ".txt", String.Format("Write PLC Tag : {0} => Time : {1}", "ST5_1ReceiveCodeResult"
                                                            , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

                var reactResult = WriteString(_view.tagMainBlock + "ST5_1ReceiveCodeActuateror", receiveActuatorCode);

                WriteLog("VerifyActuaterUpper" + _view.machine.Id + ".txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST5_1ReceiveCodeActuateror"
                                                            , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
                                                            , receiveActuatorCode));
                WriteLog("VerifyActuaterUpper" + _view.machine.Id + ".txt", String.Format("<<=================== End time : {0} ===================>>"
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
                WriteLog("VerifyActuaterUpper" + _view.machine.Id + ".txt", "");
                _view.machine = _machine;
            }
        }

        private void VerityCode(object sender, EventArgs e)
        {
            MachineModel _machine = sender as MachineModel;
            var result = _view.groupRead.Read(_view.groupRead.Items).ToList();

            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("<<=================== Start time : {0} ===================>>"
                                                                 , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

            var tagName = _view.tagMainBlock + "ST5_1CodeVerify";
            var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("Get PLC Tag value time : {0}"
                                                                 , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

            var loggings = _serviceTraceLog.GetListByItemCode(value.ToString());
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("Get Item Code : {0} from database time : {1}"
                                                                , value.ToString()
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

            WriteLog("VerifyCode" + _view.machine.Id + ".txt", "Set to by-pass station 3");
            //Defualt to by-pass station 3
            //_machine.CodeVerifyResult = 1;

            /* Set By-pass station 3 */
            if (loggings.Where(x => x.MachineId == _view.machine.Id).Count() == 0)
            {
                var newJob = loggings.Where(x => x.MachineId == 3);

                WriteLog("VerifyCodeST5_1.txt", String.Format("Get Item Code from DB : {0} ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                      CultureInfo.InvariantCulture)));

                if (newJob.Count() == 0)
                {
                    //Data not found
                    //_machine.CodeVerifyResult = 3;
                    _machine.CodeVerifyResult = 2;
                    _view.ResultnMessage = "NOK = Data not found.";
                }
                else
                {
                    var firstResult = newJob.FirstOrDefault();
                    _machine.CodeVerifyResult = firstResult.FinalResult == 1 ? 1 : 2;
                }
            }
            else
            {
                //Dupplicated
                //_machine.CodeVerifyResult = 4; 
                _machine.CodeVerifyResult = 2;
                _view.ResultnMessage = "NOK = Data dupplicated.";
            }
            /*Set By-pass station 3*/
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("Verify Code Result : {0} => Time : {1}", _machine.CodeVerifyResult.ToString()
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

            var reactResult = WriteWord(_view.tagMainBlock + "ST5_1CodeVerifyResult", _machine.CodeVerifyResult.ToString());
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("Write PLC Tag : {0}  Value = [{2}] => Complete Time : {1}"
                                                                , "ST5_1CodeVerifyResult"
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
                                                                , reactResult.ToString()));
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("<<=================== End time : {0} ===================>>"
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", "");
            _view.machine = _machine;
            _machine.RequestCodeActuater = true;
            VerityActuater(_machine, e);
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
                        {
                            machineTmp.CompletedLogging = 3;
                            WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Validate error message : {0}"
                                                               , _view.ResultnMessage));
                        }                           
                    }
                    catch (Exception ex)
                    {
                        _view.ResultnMessage = ex.Message;
                        machineTmp.CompletedLogging = 3;
                        WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Exception Message : {0}"
                                                               , _view.ResultnMessage));
                    }

                    WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Logging Result : {0} => Time : {1}"
                                                                , machineTmp.CompletedLogging.ToString()
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
                    var reactResult = WriteWord(_view.tagMainBlock + "ST5_1LoggingApp", machineTmp.CompletedLogging.ToString());
                    WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Write PLC Tag : {0}  Value = [{2}] => Complete Time : {1}"
                                                                , "ST5_1LoggingApp"
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

            var tagsEOLResult = (from tag in machineTags
                                 where tag.TypeCode == "DATA_EOL_RESULT"
                                 select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

            var tagsVanses = (from tag in machineTags
                              where tag.TypeCode == "DATA_VANSES_RESULT"
                              select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

            //Default Station and Machine
            trace.StationId = machine.StationId;
            trace.MachineId = machine.Id;
            trace.Description = "Upper  EOL laser marking and probe sensor check";

            #region Validate and Default existing log by Item Code.
            var tagName = _view.tagMainBlock + "ST5_1Code";
            var itemCode = r.Where(x => x.ItemName == tagName).FirstOrDefault().Value.ToString();
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
            #endregion

            #region Validate Data
            foreach (var item in r)
            {
                tmpMsg = string.Empty;
                if (InvalidDataTag((item.Value == null) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                    //WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Error Message 1 : {0}"
                    //                                           , errMsg));
                }
            }
            #endregion

            #region Keep Master Logging
            foreach (var item in r)
            {
                if (!invalid)
                {
                    if (item.ItemName == _view.tagMainBlock + "ST5_1Code")
                    {
                        trace.ItemCode = item.Value.ToString();

                        //var modelRunningType = _serviceTraceLog.GetListByItemCode(trace.ItemCode.ToString())
                        //                                       .Where(x => x.MachineId == 1).FirstOrDefault();

                        var passiveModel = r.Where(x => x.ItemName == _view.tagMainBlock + "ST5_1modelRunning2").FirstOrDefault().Value.ToString();
                        // result.Where(x => x.ItemName == _view.tagMainBlock + "ST5_1modelRunning2").FirstOrDefault().Value;
                        trace.ModelRunningFlag = Convert.ToInt32(passiveModel);
                    }
                    //trace.ItemCode = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_1_Product_Serial_No.")
                        trace.PartSerialNumber = item.Value.ToString();

                    if(trace.ModelRunningFlag == 1)
                    {
                        if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[0]")
                            trace.Actuator = item.Value.ToString();
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[1]")
                    {
                        trace.ProductionDate = DateTime.Now;
                        //string strDate = item.Value.ToString();

                        //if (strDate.Length > 2)
                        //{
                        //    trace.ProductionDate = GetProductionDate(strDate);
                        //}
                        //else
                        //{
                        //    invalid = true;
                        //    tmpMsg = "Production Date was not recognized as a valid DateTime.";
                        //    if (!string.IsNullOrEmpty(errMsg))
                        //        errMsg += Environment.NewLine;

                        //    errMsg += tmpMsg;
                        //}
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[2]")
                        trace.SwNumber = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[3]")
                        trace.LineErrorCounter = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[4]")
                    {
                        trace.CurrentMaximum = item.Value.ToString();
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[5]")
                    {
                        trace.OpenAngle = item.Value.ToString();
                    }

                    //Camera check Linkage
                    if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[6]")
                        trace.Attribute1 = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_1Final_Judgment")
                    {
                        if (Convert.ToInt32(item.Value) != 0)
                        {
                            trace.FinalResult = Convert.ToInt32(item.Value);
                        }
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST5_1RepairTime")
                        trace.RepairTime = Convert.ToInt32(item.Value);

                    if (item.ItemName == _view.tagMainBlock + "ST5_1modelRunning2")
                        trace.ModelRunningFlag = Convert.ToInt32(item.Value);
                }
            }
            //WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Keep Master Logging Completed"));
            #endregion

            #region Keep EOL Result             
            int i = 1;
            List<TighteningRepairModel> tRepairs = new List<TighteningRepairModel>();
            TighteningRepairModel tRepair = new TighteningRepairModel();
            TighteningResultModel tmp = new TighteningResultModel();

            if (trace.TighteningResults == null)
                trace.TighteningResults = new List<TighteningResultModel>();

            foreach (var item in r.Where(x => tagsEOLResult.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                if (!invalid)
                {
                    //No.1
                    if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[4]")
                    {
                        TighteningResultModel t = new TighteningResultModel();
                        t.No = "EOL Current(mA)";
                        t.Result = ConvertToDecimal(item.Value.ToString());
                        t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST5_1Parameter2[0]").FirstOrDefault().Value.ToString());
                        t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST5_1Parameter1[0]").FirstOrDefault().Value.ToString());
                        t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST5_1TestJudgment[0]").FirstOrDefault().Value.ToString();

                        trace.TighteningResults.Add(t);
                    }

                    //No.2
                    if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[5]")
                    {
                        TighteningResultModel t = new TighteningResultModel();
                        t.No = "Open Angle";
                        t.Result = ConvertToDecimal(item.Value.ToString());
                        t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST5_1Parameter2[1]").FirstOrDefault().Value.ToString());
                        t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST5_1Parameter1[1]").FirstOrDefault().Value.ToString());
                        t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST5_1TestJudgment[1]").FirstOrDefault().Value.ToString();

                        trace.TighteningResults.Add(t);
                    }
                }
                i++;
            }
            //WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Keep EOL Result Completed"));
            #endregion

            #region Keep Vanses Result            
            trace.CameraResults = new List<CameraResultModel>();
            foreach (var item in r.Where(x => tagsVanses.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                CameraResultModel cam = new CameraResultModel();
                if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[7]")
                {
                    cam.CameraName = "VANES Open LH";
                    cam.TestResult = item.Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[8]")
                {
                    cam.CameraName = "VANES Closed LH";
                    cam.TestResult = item.Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[9]")
                {
                    cam.CameraName = "VANES Open RH";
                    cam.TestResult = item.Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[10]")
                {
                    cam.CameraName = "VANES Closed RH";
                    cam.TestResult = item.Value.ToString();
                }

                trace.CameraResults.Add(cam);
            }
            //WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Keep Vanses Result Completed"));
            #endregion

            #region Insert/Update Logging Detail
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

                string tartgetFile;
                string errMessageFile;

                if (trace.Id == 0 && !string.IsNullOrEmpty(trace.ItemCode))
                {
                    //WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Create trace log"));
                    //WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Vanses Result : {0}", trace.CameraResults.ToString()));
                    //WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("EOL Result : {0}", trace.TighteningResults.ToString()));
                    _serviceTraceLog.Create(trace);
                    KeeppingFile(trace.PartSerialNumber, out tartgetFile, out errMessageFile);
                    result = true;
                }
                else if (trace.Id != 0 && !string.IsNullOrEmpty(trace.ItemCode))
                {
                    //WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Update trace log"));
                    //WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Vanses Result : {0}", trace.CameraResults.ToString()));
                    //WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("EOL Result : {0}", trace.TighteningResults.ToString()));
                    _serviceTraceLog.Update(trace);
                    KeeppingFile(trace.PartSerialNumber, out tartgetFile, out errMessageFile);
                    result = true;
                }
            }
            #endregion

            return result;
        }

        private bool KeeppingFile(string itemCode, out string outTargetFilePath, out string outErrorMessage)
        {
            string sourceImagePath = ConfigurationManager.AppSettings["sourcePath"].ToString();
            string targetImagePath = ConfigurationManager.AppSettings["targetPath"].ToString();

            bool result = false;
            string _errMsg = string.Empty;
            string fileName = string.Empty;
            string targetFile = string.Format(@"{0}\\{1}", targetImagePath, DateTime.Now.ToString("ddMMyyyy"));
            targetFile = RemoveSpecialCharacters(targetFile);

            //*** Create Folder
            if (!Directory.Exists(targetFile))
            {
                Directory.CreateDirectory(targetFile);
                //MessageBox.Show("Create Path : " + targetFile);
            }

            //*** Save File
            try
            {
                string filePath = sourceImagePath + "\\" + itemCode + ".JPG";
                filePath = RemoveSpecialCharacters(filePath);
                fileName = Path.GetFileName(filePath);
                File.Move(filePath, targetFile + fileName);
                result = true;
            }
            catch (Exception ex)
            {
                _errMsg = ex.Message;
                result = false;
            }

            outTargetFilePath = targetFile + fileName;
            outErrorMessage = _errMsg;
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
                //_view.groupStateRead.UpdateRate = 1000;// this isthe time between every reads from OPC server
                _view.groupStateRead.Active = false;//this must be true if you the group has to read value
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
                var mac6 = _view.machine;
                var onlineTag6 = _view.tagMainBlock + "ST5_1StatusMc";
                mac6.OnlineFlag = Convert.ToInt32(currentResult.Where(x => x.ItemName == onlineTag6).FirstOrDefault().Value);

                var reqLoggingTag6 = _view.tagMainBlock + "ST5_1ReqLogging";
                mac6.RequestLogging = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqLoggingTag6).FirstOrDefault().Value);

                var completeLoggingTag6 = _view.tagMainBlock + "ST5_1LoggingApp";
                mac6.CompletedLogging = Convert.ToInt32(currentResult.Where(x => x.ItemName == completeLoggingTag6).FirstOrDefault().Value);

                var reqVerifyTag6 = _view.tagMainBlock + "ST5_1ReqChkCodeVerify";
                mac6.RequestVerifyCode = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqVerifyTag6).FirstOrDefault().Value);

                var verifyResultTag6 = _view.tagMainBlock + "ST5_1CodeVerifyResult";
                mac6.CodeVerifyResult = Convert.ToInt32(currentResult.Where(x => x.ItemName == verifyResultTag6).FirstOrDefault().Value);

                var reqActuaterTag6 = _view.tagMainBlock + "ST5_1ReqCodeActuater";
                mac6.RequestCodeActuater = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqActuaterTag6).FirstOrDefault().Value);

                var reqActuaterResultTag6 = _view.tagMainBlock + "ST5_1ReceiveCodeResult";
                mac6.ActuatorResult = Convert.ToInt32(currentResult.Where(x => x.ItemName == reqActuaterResultTag6).FirstOrDefault().Value);


                _view.machine = mac6;
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
            {
                WriteLog("ConvertToDecimal_Machine_" + _view.machine.Id + ".txt", String.Format("Input string is Null or Empty at time {0}"
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
                number = "0";                
            }                

            bool result = decimal.TryParse(number, out myDecimal);

            if (result)
                return myDecimal;
            else
            {
                WriteLog("ConvertToDecimal_Machine_" + _view.machine.Id + ".txt", String.Format("Input string is [{0}] at time {1}"
                                                                , number
                                                                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
                return 0;
            }                
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
