﻿using Opc.Da;
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

namespace Trace.OpcHandlerMachine02.Presenters
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
            _view.InterLock += InterLock;
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
                                                                 , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

            var tagName = _view.tagMainBlock + "ST2CodeVerify";
            var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("Get PLC Tag value time : {0}"
                                                                 , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

            var loggings = _serviceTraceLog.GetListByItemCode(value.ToString());
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("Get Item Code : {0} from database time : {1}"
                                                                , value.ToString()
                                                                , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

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
                                                                , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));

            var reactResult = WriteWord(_view.tagMainBlock + "ST2CodeVerifyResult", _machine.CodeVerifyResult.ToString());
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("Write PLC Tag : {0}  Value = [{2}] => Complete Time : {1}"
                                                                , "ST2CodeVerifyResult"
                                                                , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
                                                                , reactResult.ToString()));
            WriteLog("VerifyCode" + _view.machine.Id + ".txt", String.Format("<<=================== End time : {0} ===================>>"
                                                                , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
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
                                                                , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
                        keepLog = LoggingData(r, machine, machineTags);
                        if (keepLog)
                        {
                            machineTmp.CompletedLogging = 1;
                            WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Insert to Database complete time : {0}"
                                                               , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
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
                                                                , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
                    var reactResult = WriteWord(_view.tagMainBlock + "ST2LoggingApp", machineTmp.CompletedLogging.ToString());
                    WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("Write PLC Tag : {0}  Value = [{2}] => Complete Time : {1}"
                                                                , "ST2LoggingApp"
                                                                , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
                                                                , machineTmp.CompletedLogging.ToString()));
                    WriteLog("KeepLogging" + _view.machine.Id + ".txt", String.Format("<<=================== End time : {0} ===================>>"
                                                                , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
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

            //Default Station and Machine
            trace.StationId = machine.StationId;
            trace.MachineId = machine.Id;
            trace.ProductionDate = DateTime.Now;

            #region  Validate and Default existing log by Item Code.
            var tagName = _view.tagMainBlock + "ST2Code";
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

            #region Keep Master Logging
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
                    if (item.ItemName == _view.tagMainBlock + "ST2Code")
                        trace.ItemCode = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST2Final_Judgment")
                    {
                        if (Convert.ToInt32(item.Value) != 0)
                        {
                            trace.FinalResult = Convert.ToInt32(item.Value);
                        }
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST2RepairTime")
                        trace.RepairTime = Convert.ToInt32(item.Value);
                }
            }
            #endregion

            #region Keep Part Assemblies
            trace.PartAssemblies = new List<PartAssemblyModel>();
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
                    if (item.ItemName == _view.tagMainBlock + "ST2PartSerialNo[0]")
                    {
                        part.LineNumber = 1;
                        part.PartName = "Z support LH";
                        part.SerialNumber = item.Value.ToString();
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST2PartSerialNo[1]")
                    {
                        part.LineNumber = 2;
                        part.PartName = "Z support RH";
                        part.SerialNumber = item.Value.ToString();
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST2PartSerialNo[2]")
                    {
                        part.LineNumber = 3;
                        part.PartName = "Blind vane RH1";
                        part.SerialNumber = item.Value.ToString();
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST2PartSerialNo[3]")
                    {
                        part.LineNumber = 4;
                        part.PartName = "Blind vane RH2";
                        part.SerialNumber = item.Value.ToString();
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST2PartSerialNo[4]")
                    {
                        part.LineNumber = 5;
                        part.PartName = "Blind vane LH1";
                        part.SerialNumber = item.Value.ToString();
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST2PartSerialNo[5]")
                    {
                        part.LineNumber = 6;
                        part.PartName = "Blind vane LH2";
                        part.SerialNumber = item.Value.ToString();
                    }

                    trace.PartAssemblies.Add(part);
                }
            }
            #endregion

            #region Keep Tightening
            //int tigtheingNumber = 0;
            List<TighteningRepairModel> tRepairs = new List<TighteningRepairModel>();
            TighteningRepairModel tRepair = new TighteningRepairModel();
            TighteningResultModel tmp = new TighteningResultModel();

            if (trace.TighteningResults == null)
                trace.TighteningResults = new List<TighteningResultModel>();

            foreach (var item in r.Where(x => tagsTightening.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                tmpMsg = string.Empty;
                if (!invalid)
                {
                    //No.1
                    if (item.ItemName == _view.tagMainBlock + "ST2TestResult[0]")
                    {
                        TighteningResultModel t = new TighteningResultModel();
                        t.No = "Hollow Nut Screw L";
                        t.Result = ConvertToDecimal(item.Value.ToString());
                        t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[0]").FirstOrDefault().Value.ToString());
                        t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[0]").FirstOrDefault().Value.ToString());
                        t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[0]").FirstOrDefault().Value.ToString());
                        t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[0]").FirstOrDefault().Value.ToString();

                        //JointsControlAngle Result 
                        t.JointResult = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestResult[5]").FirstOrDefault().Value.ToString());
                        t.JointMin = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[5]").FirstOrDefault().Value.ToString());
                        t.JointMax = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[5]").FirstOrDefault().Value.ToString());
                        t.JointTarget = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[5]").FirstOrDefault().Value.ToString());
                        t.JointTestResult = t.TestResult;// r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[5]").FirstOrDefault().Value.ToString();

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
                    if (item.ItemName == _view.tagMainBlock + "ST2TestResult[1]")
                    {
                        TighteningResultModel t = new TighteningResultModel();
                        t.No = "Hollow Nut Screw R";
                        t.Result = ConvertToDecimal(item.Value.ToString());
                        t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[1]").FirstOrDefault().Value.ToString());
                        t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[1]").FirstOrDefault().Value.ToString());
                        t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[1]").FirstOrDefault().Value.ToString());
                        t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[1]").FirstOrDefault().Value.ToString();

                        //JointsControlAngle Result 
                        t.JointResult = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestResult[6]").FirstOrDefault().Value.ToString());
                        t.JointMin = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[6]").FirstOrDefault().Value.ToString());
                        t.JointMax = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[6]").FirstOrDefault().Value.ToString());
                        t.JointTarget = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[6]").FirstOrDefault().Value.ToString());
                        t.JointTestResult = t.TestResult; // r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[6]").FirstOrDefault().Value.ToString();

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

                    //No.3
                    if (item.ItemName == _view.tagMainBlock + "ST2TestResult[2]")
                    {
                        TighteningResultModel t = new TighteningResultModel();
                        t.No = "Ejot Screw L";
                        t.Result = ConvertToDecimal(item.Value.ToString());
                        t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[2]").FirstOrDefault().Value.ToString());
                        t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[2]").FirstOrDefault().Value.ToString());
                        t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[2]").FirstOrDefault().Value.ToString());
                        t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[2]").FirstOrDefault().Value.ToString();

                        //JointsControlAngle Result 
                        t.JointResult = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestResult[7]").FirstOrDefault().Value.ToString());
                        t.JointMin = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[7]").FirstOrDefault().Value.ToString());
                        t.JointMax = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[7]").FirstOrDefault().Value.ToString());
                        t.JointTarget = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[7]").FirstOrDefault().Value.ToString());
                        t.JointTestResult = t.TestResult; // r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[7]").FirstOrDefault().Value.ToString();

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

                    //No.4
                    if (item.ItemName == _view.tagMainBlock + "ST2TestResult[3]")
                    {
                        TighteningResultModel t = new TighteningResultModel();
                        t.No = "Ejot Screw R";
                        t.Result = ConvertToDecimal(item.Value.ToString());
                        t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[3]").FirstOrDefault().Value.ToString());
                        t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[3]").FirstOrDefault().Value.ToString());
                        t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[3]").FirstOrDefault().Value.ToString());
                        t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[3]").FirstOrDefault().Value.ToString();

                        //JointsControlAngle Result 
                        t.JointResult = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestResult[8]").FirstOrDefault().Value.ToString());
                        t.JointMin = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[8]").FirstOrDefault().Value.ToString());
                        t.JointMax = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[8]").FirstOrDefault().Value.ToString());
                        t.JointTarget = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[8]").FirstOrDefault().Value.ToString());
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

                    //No.5
                    /*if (item.ItemName == _view.tagMainBlock + "ST2TestResult[4]")
                    {
                        TighteningResultModel t = new TighteningResultModel();
                        t.No = "Hollow nuts tightening depth";
                        t.Result = ConvertToDecimal(item.Value.ToString());
                        t.Min = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[4]").FirstOrDefault().Value.ToString());
                        t.Max = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[4]").FirstOrDefault().Value.ToString());
                        t.Target = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[4]").FirstOrDefault().Value.ToString());
                        t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[4]").FirstOrDefault().Value.ToString();

                        //JointsControlAngle Result 
                        t.JointResult = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestResult[9]").FirstOrDefault().Value.ToString());
                        t.JointMin = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[9]").FirstOrDefault().Value.ToString());
                        t.JointMax = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[9]").FirstOrDefault().Value.ToString());
                        t.JointTarget = ConvertToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[9]").FirstOrDefault().Value.ToString());
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
                    }*/
                }
                // i++;
            }
            #endregion

            #region Insert/Update Logging Detail
            if (invalid)
            {
                var mac = _view.machine;
                mac.MessageResult = errMsg;
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
                            cRepair.TighteningResultId = item.Id;
                            _serviceTigtheningRepair.Create(cRepair);
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
                                cRepair.TighteningResultId = item.Id;
                                _serviceTigtheningRepair.Create(cRepair);
                            }
                        }
                    }
                    result = true;
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
            else
            {
                _view.ResultnMessage = "Inter Lock completed.";
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
                var mac2 = _view.machine;
                var onlineTag2 = _view.tagMainBlock + "ST2StatusMc";
                mac2.OnlineFlag = Convert.ToInt32(currentResult.Where(x => x.ItemName == onlineTag2).FirstOrDefault().Value);

                var reqLoggingTag2 = _view.tagMainBlock + "ST2ReqLogging";
                mac2.RequestLogging = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqLoggingTag2).FirstOrDefault().Value);

                var completeLoggingTag2 = _view.tagMainBlock + "ST2LoggingApp";
                mac2.CompletedLogging = Convert.ToInt32(currentResult.Where(x => x.ItemName == completeLoggingTag2).FirstOrDefault().Value);

                var reqVerifyTag2 = _view.tagMainBlock + "ST2ReqChkCodeVerify";
                mac2.RequestVerifyCode = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqVerifyTag2).FirstOrDefault().Value);

                var verifyResultTag2 = _view.tagMainBlock + "ST2CodeVerifyResult";
                mac2.CodeVerifyResult = Convert.ToInt32(currentResult.Where(x => x.ItemName == verifyResultTag2).FirstOrDefault().Value);

                int receivedData2;
                var positionTag2 = _view.tagMainBlock + "ST2TestResult[19]";
                var succes2 = Int32.TryParse(currentResult.Where(x => x.ItemName == positionTag2).FirstOrDefault().Value.ToString(), out receivedData2);
                if (!succes2)
                    receivedData2 = 0;
                mac2.TighteningPosition = receivedData2;

                _view.machine = mac2;

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
