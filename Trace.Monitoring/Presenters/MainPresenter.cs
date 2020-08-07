using Google.Protobuf.WellKnownTypes;
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

namespace Trace.Monitoring.Presenters
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

        private readonly IMainView _view;

        public MainPresenter(IMainView view)
        {
            _view = view;

            _view.FormLoad += FormLoad;
            _view.Connect_Click += Connect;
            _view.Disconnect_Click += Disconnect_Click;
            _view.InterLock += InterLock;
            _view.MakeReady += MakeReady;
            _view.KeepLogging += KeepLogging;
            _view.CompleteAction += CompleteAction;
            _view.VerityCode += VerityCode;
            _view.VerityActuater += VerityActuater;
            _view.RefreshData += RefreshData;
        }

        private void RefreshData(object sender, EventArgs e)
        {
            //WriteLog("TracebilityLogs.txt", String.Format("Refresh action : {0}", DateTime.Now));
            //WriteLog("TracebilityLogs.txt", String.Format("Start refresh time : {0}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
            //                                CultureInfo.InvariantCulture)));
            if (_view.connectedPlc)
                LoadCurrentValue(_view.groupRead);

            //WriteLog("TracebilityLogs.txt", String.Format("End refresh time : {0}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
            //                                CultureInfo.InvariantCulture)));
            //WriteLog("TracebilityLogs.txt", String.Format("----------------------------------------------------------------------------------"));
        }

        private void VerityActuater(object sender, EventArgs e)
        {
            MachineModel _machine = sender as MachineModel;
            var grpReadResult = _view.groupRead.Read(_view.groupRead.Items).ToList();

            //machine 6
            if (_machine.Id == 6)
            {
                var tagItemCode = _view.tagMainBlock + "ST5_1Code";
                //var tagName = _view.tagMainBlock + "ST5_1ReceiveCodeActuateror";

                var itemCode = grpReadResult.Where(x => x.ItemName == tagItemCode).FirstOrDefault().Value;
                //var actuater = grpReadResult.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                WriteLog("VerifyActuaterUpper.txt", String.Format("Item Code : {0} => Time : {1}", itemCode, DateTime.Now));

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

                WriteLog("VerifyActuaterUpper.txt", String.Format("Actuator Result : {0} => Time : {1}", _machine.ActuatorResult, DateTime.Now));
                _view.machine6 = _machine;
                ReactCompleteLog(_view.tagMainBlock + "ST5_1ReceiveCodeResult", _machine.ActuatorResult);
                WriteLog("VerifyActuaterUpper.txt", String.Format("Write PLC Tag : {0} => Time : {1}", "ST5_1ReceiveCodeResult", DateTime.Now));
                var reactResult = ReactDataTag(_view.tagMainBlock + "ST5_1ReceiveCodeActuateror", receiveActuatorCode);
                WriteLog("VerifyActuaterUpper.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST5_1ReceiveCodeActuateror"
                                                            , DateTime.Now
                                                            , reactResult.ToString()));
            }

            //machine 7
            if (_machine.Id == 7)
            {
                var tagItemCode = _view.tagMainBlock + "ST5_2Code";
                //var tagName = _view.tagMainBlock + "ST5_2ReceiveCodeActuateror";

                var itemCode = grpReadResult.Where(x => x.ItemName == tagItemCode).FirstOrDefault().Value;
                //var actuater = grpReadResult.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                WriteLog("VerifyActuaterLower.txt", String.Format("Item Code : {0} => Time : {1}", itemCode, DateTime.Now));

                var result = _serviceTraceLog.GetListByItemCode(itemCode.ToString());
                var loggings = result.Where(x => x.MachineId == 5);
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

                    if (partAssblies != null)
                    {
                        var actuaterResult = partAssblies.Where(x => x.PartName == "LWR Actuator P/N");

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

                WriteLog("VerifyActuaterLower.txt", String.Format("Actuator Result : {0} => Time : {1}", _machine.ActuatorResult.ToString(), DateTime.Now));
                _view.machine7 = _machine;
                ReactCompleteLog(_view.tagMainBlock + "ST5_2ReceiveCodeResult", _machine.ActuatorResult);
                WriteLog("VerifyActuaterLower.txt", String.Format("Write PLC Tag : {0} => Time : {1}", "ST5_2ReceiveCodeResult", DateTime.Now));
                var reactResult = ReactDataTag(_view.tagMainBlock + "ST5_2ReceiveCodeActuateror", receiveActuatorCode);
                WriteLog("VerifyActuaterLower.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST5_2ReceiveCodeActuateror"
                                                            , DateTime.Now
                                                            , reactResult.ToString()));
            }

        }

        private void VerityCode(object sender, EventArgs e)
        {
            MachineModel _machine = sender as MachineModel;
            var result = _view.groupRead.Read(_view.groupRead.Items).ToList();

            //machine 1
            if (_machine.Id == 1)
            {
                var tagName = _view.tagMainBlock + "ST1CodeVerify";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                WriteLog("VerifyCodeST1.txt", String.Format("Item Code : {0} => Time : {1}", value.ToString(), DateTime.Now));

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString())
                                               .Where(x => x.MachineId == 1);

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

                WriteLog("VerifyCodeST1.txt", String.Format("Verify Code Result : {0} => Time : {1}", _machine.CodeVerifyResult.ToString(), DateTime.Now));
                _view.machine1 = _machine;
                var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST1CodeVerifyResult", _machine.CodeVerifyResult);
                WriteLog("VerifyCodeST1.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST1CodeVerifyResult"
                                                            , DateTime.Now
                                                            , reactResult.ToString()));
            }

            //machine 2
            if (_machine.Id == 2)
            {
                var tagName = _view.tagMainBlock + "ST2CodeVerify";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                WriteLog("VerifyCodeST2.txt", String.Format("Item Code : {0} => Time : {1}", value.ToString(), DateTime.Now));

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString())
                                               .Where(x => x.MachineId == 2);

                if (loggings.Count() == 0)
                {
                    var newJob = _serviceTraceLog.GetListByItemCode(value.ToString())
                                                                   .Where(x => x.MachineId == 1);
                    if (newJob.Count() == 0)
                    {
                        //Data not found
                        //_machine.CodeVerifyResult = 3;/
                        _machine.CodeVerifyResult = 2;
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
                }
                WriteLog("VerifyCodeST2.txt", String.Format("Verify Code Result : {0} => Time : {1}", _machine.CodeVerifyResult.ToString(), DateTime.Now));
                _view.machine2 = _machine;
                var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST2CodeVerifyResult", _machine.CodeVerifyResult);
                WriteLog("VerifyCodeST2.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST2CodeVerifyResult"
                                                            , DateTime.Now
                                                            , reactResult.ToString()));
            }

            //machine 3
            if (_machine.Id == 3)
            {
                var tagName = _view.tagMainBlock + "ST3_1CodeVerify";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                WriteLog("VerifyCodeST3_1.txt", String.Format("Item Code : {0} => Completed time : {1}", value.ToString(), DateTime.Now));

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString())
                               .Where(x => x.MachineId == 3);

                if (loggings.Count() == 0)
                {
                    var newJob = _serviceTraceLog.GetListByItemCode(value.ToString())
                                                                   .Where(x => x.MachineId == 2);
                    if (newJob.Count() == 0)
                    {
                        //Data not found
                        //_machine.CodeVerifyResult = 3;
                        _machine.CodeVerifyResult = 2;
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
                }

                WriteLog("VerifyCodeST3_1.txt", String.Format("Verify Code Result : {0} => Time : {1}", _machine.CodeVerifyResult.ToString(), DateTime.Now));
                _view.machine3 = _machine;
                var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST3_1CodeVerifyResult", _machine.CodeVerifyResult);
                WriteLog("VerifyCodeST3_1.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST3_1CodeVerifyResult"
                                                            , DateTime.Now
                                                            , reactResult.ToString()));
            }

            //machine 4
            if (_machine.Id == 4)
            {
                var tagName = _view.tagMainBlock + "ST3_2CodeVerify";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                WriteLog("VerifyCodeST3_2.txt", String.Format("Item Code : {0} => Time : {1}", value.ToString(), DateTime.Now));

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString())
                               .Where(x => x.MachineId == 4);

                if (loggings.Count() == 0)
                {
                    var newJob = _serviceTraceLog.GetListByItemCode(value.ToString())
                                                                   .Where(x => x.MachineId == 5);
                    if (newJob.Count() == 0)
                    {
                        //Data not found
                        //_machine.CodeVerifyResult = 3;
                        _machine.CodeVerifyResult = 2;
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
                }

                WriteLog("VerifyCodeST3_2.txt", String.Format("Verify Code Result : {0} => Time : {1}", _machine.CodeVerifyResult.ToString(), DateTime.Now));
                _view.machine4 = _machine;
                var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST3_2CodeVerifyResult", _machine.CodeVerifyResult);
                WriteLog("VerifyCodeST3_2.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST3_2CodeVerifyResult"
                                                            , DateTime.Now
                                                            , reactResult.ToString()));
            }

            //machine 5
            if (_machine.Id == 5)
            {
                WriteLog("VerifyCodeST4.txt", String.Format("Start verify time : {0}", TimeSpan.TicksPerMillisecond.ToString()));
                WriteLog("VerifyCodeST4.txt", String.Format("Start verify time : {0}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                            CultureInfo.InvariantCulture)));
                var tagName = _view.tagMainBlock + "ST4CodeVerify";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;
                WriteLog("VerifyCodeST4.txt", String.Format("Get PLC TAG value time : {0}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                            CultureInfo.InvariantCulture)));
                WriteLog("VerifyCodeST4.txt", String.Format("Item Code : {0} => Time : {1}", value.ToString(), DateTime.Now));

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString())
                                               .Where(x => x.MachineId == 5);

                WriteLog("VerifyCodeST4.txt", String.Format("Get Database result time : {0}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                            CultureInfo.InvariantCulture)));
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

                WriteLog("VerifyCodeST4.txt", String.Format("Verify Code Result : {0} => Time : {1}", _machine.CodeVerifyResult.ToString(), DateTime.Now));
                var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST4CodeVerifyResult", _machine.CodeVerifyResult);
                WriteLog("VerifyCodeST4.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST4CodeVerifyResult"
                                                            , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
                                                            , reactResult.ToString()));

                _view.machine5 = _machine;
            }

            //machine 6
            if (_machine.Id == 6)
            {
                var tagName = _view.tagMainBlock + "ST5_1CodeVerify";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                WriteLog("VerifyCodeST5_1.txt", String.Format("Item Code : {0} => Time : {1}", value.ToString(), DateTime.Now));

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString())
                               .Where(x => x.MachineId == 6);

                if (loggings.Count() == 0)
                {
                    //var newJob = _serviceTraceLog.GetListByItemCode(value.ToString())
                    //                                               .Where(x => x.MachineId == 3);

                    //Skip Station 3
                    var newJob = _serviceTraceLog.GetListByItemCode(value.ToString())
                                                                   .Where(x => x.MachineId == 2);

                    if (newJob.Count() == 0)
                    {
                        //Data not found
                        //_machine.CodeVerifyResult = 3;
                        _machine.CodeVerifyResult = 2;
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
                }

                WriteLog("VerifyCodeST5_1.txt", String.Format("Verify Code Result : {0} => Time : {1}", _machine.CodeVerifyResult.ToString(), DateTime.Now));
                _view.machine6 = _machine;
                var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST5_1CodeVerifyResult", _machine.CodeVerifyResult);
                WriteLog("VerifyCodeST5_1.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST5_1CodeVerifyResult"
                                                            , DateTime.Now
                                                            , reactResult.ToString()));
            }

            //machine 7
            if (_machine.Id == 7)
            {
                WriteLog("VerifyCodeST5_2.txt", String.Format("Start verify time : {0}", TimeSpan.TicksPerMillisecond.ToString()));
                WriteLog("VerifyCodeST5_2.txt", String.Format("Start verify time : {0}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                            CultureInfo.InvariantCulture)));
                var tagName = _view.tagMainBlock + "ST5_2CodeVerify";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;
                WriteLog("VerifyCodeST5_2.txt", String.Format("Get PLC TAG value time : {0}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                           CultureInfo.InvariantCulture)));

                WriteLog("VerifyCodeST5_2.txt", String.Format("Item Code : {0} => Time : {1}", value.ToString(), DateTime.Now));

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString())
                               .Where(x => x.MachineId == 7);

                WriteLog("VerifyCodeST5_2.txt", String.Format("Get Database result time : {0}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                            CultureInfo.InvariantCulture)));
                if (loggings.Count() == 0)
                {
                    var newJob = _serviceTraceLog.GetListByItemCode(value.ToString())
                                                                   .Where(x => x.MachineId == 5);
                    if (newJob.Count() == 0)
                    {
                        //Data not found
                        //_machine.CodeVerifyResult = 3;
                        _machine.CodeVerifyResult = 2;
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
                }

                WriteLog("VerifyCodeST5_2.txt", String.Format("Verify Code Result : {0} => Time : {1}", _machine.CodeVerifyResult.ToString(), DateTime.Now));
                _view.machine7 = _machine;
                var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST5_2CodeVerifyResult", _machine.CodeVerifyResult);
                WriteLog("VerifyCodeST5_2.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST5_2CodeVerifyResult"
                                                            , DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
                                                            , reactResult.ToString()));
            }
        }

        private void CompleteAction(object sender, EventArgs e)
        {
            Button but = sender as Button;
            var result = _view.groupRead.Read(_view.groupRead.Items).ToList();
            if (but.Name == "butCompletedLogging1")
            {
                var tagName = _view.tagMainBlock + "ST1Code";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString());
                if (loggings.Count() > 0)
                {
                    ReactCompleteLog(_view.tagMainBlock + "ST1LoggingApp", 1);
                }
                else
                {
                    MachineModel machine = _serviceMachine.GetByID(1);

                    var machineTags = _servicePLCTag.GetAll().ToList().Where(x => x.MachineId == machine.Id);
                    var tags = (from tag in machineTags
                                where tag.MachineId == machine.Id
                                select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                    var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                    bool keepLog = KeepLogForMachine1(r, machine, machineTags);
                    if (keepLog)
                    {
                        ReactCompleteLog(_view.tagMainBlock + "ST1LoggingApp", 1);
                    }
                }
            }

            if (but.Name == "butCompletedLogging2")
            {
                var tagName = _view.tagMainBlock + "ST2Code";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString());
                if (loggings.Count() > 0)
                {
                    ReactCompleteLog(_view.tagMainBlock + "ST2LoggingApp", 1);
                }
                else
                {
                    MachineModel machine = _serviceMachine.GetByID(2);

                    var machineTags = _servicePLCTag.GetAll().ToList().Where(x => x.MachineId == machine.Id);
                    var tags = (from tag in machineTags
                                where tag.MachineId == machine.Id
                                select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                    var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                    bool keepLog = KeepLogForMachine2(r, machine, machineTags);
                    if (keepLog)
                    {
                        ReactCompleteLog(_view.tagMainBlock + "ST2LoggingApp", 1);
                    }
                }
            }

            if (but.Name == "butCompletedLogging3")
            {
                var tagName = _view.tagMainBlock + "ST3_1Code";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString());
                if (loggings.Count() > 0)
                {
                    ReactCompleteLog(_view.tagMainBlock + "ST3_1LoggingApp", 1);
                }
                else
                {
                    MachineModel machine = _serviceMachine.GetByID(3);

                    var machineTags = _servicePLCTag.GetAll().ToList().Where(x => x.MachineId == machine.Id);
                    var tags = (from tag in machineTags
                                where tag.MachineId == machine.Id
                                select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                    var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                    bool keepLog = KeepLogForMachine3(r, machine);
                    if (keepLog)
                    {
                        ReactCompleteLog(_view.tagMainBlock + "ST3_1LoggingApp", 1);
                    }
                }
            }

            if (but.Name == "butCompletedLogging4")
            {
                var tagName = _view.tagMainBlock + "ST3_2Code";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString());
                if (loggings.Count() > 0)
                {
                    ReactCompleteLog(_view.tagMainBlock + "ST3_2LoggingApp", 1);
                }
                else
                {
                    MachineModel machine = _serviceMachine.GetByID(4);

                    var machineTags = _servicePLCTag.GetAll().ToList().Where(x => x.MachineId == machine.Id);
                    var tags = (from tag in machineTags
                                where tag.MachineId == machine.Id
                                select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                    var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                    bool keepLog = KeepLogForMachine4(r, machine);
                    if (keepLog)
                    {
                        ReactCompleteLog(_view.tagMainBlock + "ST3_2LoggingApp", 1);
                    }
                }
            }

            if (but.Name == "butCompletedLogging5")
            {
                //var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST4LoggingApp", 1);
                //ReactCompleteLog(_view.tagMainBlock + "ST4LoggingApp", 1);
                var tagName = _view.tagMainBlock + "ST4Code";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString());
                if (loggings.Count() > 0)
                {
                    ReactCompleteLog(_view.tagMainBlock + "ST4LoggingApp", 1);
                }
                else
                {
                    MachineModel machine = _serviceMachine.GetByID(5);

                    var machineTags = _servicePLCTag.GetAll().ToList().Where(x => x.MachineId == machine.Id);
                    var tags = (from tag in machineTags
                                where tag.MachineId == machine.Id
                                select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                    var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                    bool keepLog = KeepLogForMachine5(r, machine, machineTags);
                    if (keepLog)
                    {
                        ReactCompleteLog(_view.tagMainBlock + "ST4LoggingApp", 1);
                    }
                }
            }

            if (but.Name == "butCompletedLogging6")
            {
                var tagName = _view.tagMainBlock + "ST5_1Code";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString());
                if (loggings.Count() > 0)
                {
                    ReactCompleteLog(_view.tagMainBlock + "ST5_1LoggingApp", 1);
                }
                else
                {
                    MachineModel machine = _serviceMachine.GetByID(6);

                    var machineTags = _servicePLCTag.GetAll().ToList().Where(x => x.MachineId == machine.Id);
                    var tags = (from tag in machineTags
                                where tag.MachineId == machine.Id
                                select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                    var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                    bool keepLog = KeepLogForMachine6(r, machine, machineTags);
                    if (keepLog)
                    {
                        ReactCompleteLog(_view.tagMainBlock + "ST5_1LoggingApp", 1);
                    }
                }
            }

            if (but.Name == "butCompletedLogging7")
            {
                var tagName = _view.tagMainBlock + "ST5_2Code";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString());
                if (loggings.Count() > 0)
                {
                    ReactCompleteLog(_view.tagMainBlock + "ST5_2LoggingApp", 1);
                }
                else
                {
                    MachineModel machine = _serviceMachine.GetByID(7);

                    var machineTags = _servicePLCTag.GetAll().ToList().Where(x => x.MachineId == machine.Id);
                    var tags = (from tag in machineTags
                                where tag.MachineId == machine.Id
                                select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                    var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                    bool keepLog = KeepLogForMachine7(r, machine, machineTags);
                    if (keepLog)
                    {
                        ReactCompleteLog(_view.tagMainBlock + "ST5_2LoggingApp", 1);
                    }
                }
            }
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
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = subscipt.ToList();
                var machineTags = _servicePLCTag.GetAll().ToList().Where(x => x.MachineId == machine.Id);
                var tags = (from tag in machineTags
                            where tag.MachineId == machine.Id
                            select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                #region Station1
                if (machine.Id == 1)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine1;
                    machineTmp.MessageResult = string.Empty;

                    try
                    {
                        WriteLog("KeepLoggingST1.txt", String.Format("<<=================== Start time : {0} ===================>>", DateTime.Now));
                        keepLog = KeepLogForMachine1(r, machine, machineTags);
                        if (keepLog)
                            machineTmp.CompletedLogging = 1;
                        else
                            machineTmp.CompletedLogging = 3;
                    }
                    catch (Exception ex)
                    {
                        machineTmp.MessageResult = ex.Message;
                        machineTmp.CompletedLogging = 3;
                        WriteLog("KeepLoggingST1.txt", "<<=================== Begin Error Exception ===================>>");
                        WriteLog("KeepLoggingST1.txt", ex.Message);
                        WriteLog("KeepLoggingST1.txt", "<<=================== End Error Exception ===================>>");
                    }

                    WriteLog("KeepLoggingST1.txt", String.Format("LoggingApp Result : {0} => Time : {1}"
                                                            , machineTmp.CompletedLogging.ToString()
                                                            , DateTime.Now));
                    var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST1LoggingApp", machineTmp.CompletedLogging);
                    WriteLog("KeepLoggingST1.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST1LoggingApp"
                                                            , DateTime.Now
                                                            , reactResult.ToString()));
                    _view.machine1 = machineTmp;
                }
                #endregion

                #region Station2
                if (machine.Id == 2)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine2;
                    machineTmp.MessageResult = string.Empty;

                    try
                    {
                        WriteLog("KeepLoggingST2.txt", String.Format("<<=================== Start time : {0} ===================>>", DateTime.Now));
                        keepLog = KeepLogForMachine2(r, machine, machineTags);
                        if (keepLog)
                            machineTmp.CompletedLogging = 1;
                        else
                            machineTmp.CompletedLogging = 3;
                    }
                    catch (Exception ex)
                    {
                        machineTmp.MessageResult = ex.Message;
                        machineTmp.CompletedLogging = 3;
                        WriteLog("KeepLoggingST2.txt", "<<=================== Begin Error Exception ===================>>");
                        WriteLog("KeepLoggingST2.txt", ex.Message);
                        WriteLog("KeepLoggingST2.txt", "<<=================== End Error Exception ===================>>");
                    }

                    WriteLog("KeepLoggingST2.txt", String.Format("LoggingApp Result : {0} => Time : {1}"
                                                            , machineTmp.CompletedLogging.ToString()
                                                            , DateTime.Now));
                    var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST2LoggingApp", machineTmp.CompletedLogging);
                    WriteLog("KeepLoggingST2.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST2LoggingApp"
                                                            , DateTime.Now
                                                            , reactResult.ToString()));
                    _view.machine2 = machineTmp;
                }
                #endregion

                #region Station3
                if (machine.Id == 3)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine3;
                    _view.machine3.MessageResult = string.Empty;

                    try
                    {
                        WriteLog("KeepLoggingST3_1.txt", String.Format("<<=================== Start time : {0} ===================>>", DateTime.Now));
                        keepLog = KeepLogForMachine3(r, machine);
                        if (keepLog)
                            machineTmp.CompletedLogging = 1;
                        else
                            machineTmp.CompletedLogging = 3;
                    }
                    catch (Exception ex)
                    {
                        _view.machine3.MessageResult = ex.Message;
                        machineTmp.CompletedLogging = 3;
                        WriteLog("KeepLoggingST3_1.txt", "<<=================== Begin Error Exception ===================>>");
                        WriteLog("KeepLoggingST3_1.txt", ex.Message);
                        WriteLog("KeepLoggingST3_1.txt", "<<=================== End Error Exception ===================>>");
                    }

                    WriteLog("KeepLoggingST3_1.txt", String.Format("LoggingApp Result : {0} => Time : {1}"
                                                            , machineTmp.CompletedLogging.ToString()
                                                            , DateTime.Now));
                    var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST3_1LoggingApp", machineTmp.CompletedLogging);
                    WriteLog("KeepLoggingST3_1.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST3_1LoggingApp"
                                                            , DateTime.Now
                                                            , reactResult.ToString()));
                    _view.machine3 = machineTmp;
                }

                if (machine.Id == 4)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine4;
                    _view.machine4.MessageResult = string.Empty;

                    try
                    {
                        WriteLog("KeepLoggingST3_2.txt", String.Format("<<=================== Start time : {0} ===================>>", DateTime.Now));
                        keepLog = KeepLogForMachine4(r, machine);
                        if (keepLog)
                            machineTmp.CompletedLogging = 1;
                        else
                            machineTmp.CompletedLogging = 3;
                    }
                    catch (Exception ex)
                    {
                        _view.machine4.MessageResult = ex.Message;
                        machineTmp.CompletedLogging = 3;
                        WriteLog("KeepLoggingST3_2.txt", "<<=================== Begin Error Exception ===================>>");
                        WriteLog("KeepLoggingST3_2.txt", ex.Message);
                        WriteLog("KeepLoggingST3_2.txt", "<<=================== End Error Exception ===================>>");
                    }

                    WriteLog("KeepLoggingST3_2.txt", String.Format("LoggingApp Result : {0} => Time : {1}"
                                                            , machineTmp.CompletedLogging.ToString()
                                                            , DateTime.Now));
                    var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST3_2LoggingApp", machineTmp.CompletedLogging);
                    WriteLog("KeepLoggingST3_2.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST3_2LoggingApp"
                                                            , DateTime.Now
                                                            , reactResult.ToString()));
                    _view.machine4 = machineTmp;
                }
                #endregion

                #region Station4
                if (machine.Id == 5)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine5;
                    _view.machine5.MessageResult = string.Empty;

                    try
                    {
                        WriteLog("KeepLoggingST4.txt", String.Format("<<=================== Start time : {0} ===================>>", DateTime.Now));
                        keepLog = KeepLogForMachine5(r, machine, machineTags);
                        if (keepLog)
                            machineTmp.CompletedLogging = 1;
                        else
                            machineTmp.CompletedLogging = 3;
                    }
                    catch (Exception ex)
                    {
                        _view.machine5.MessageResult = ex.Message;
                        machineTmp.CompletedLogging = 3;
                        WriteLog("KeepLoggingST4.txt", "<<=================== Begin Error Exception ===================>>");
                        WriteLog("KeepLoggingST4.txt", ex.Message);
                        WriteLog("KeepLoggingST4.txt", "<<=================== End Error Exception ===================>>");
                    }

                    WriteLog("KeepLoggingST4.txt", String.Format("LoggingApp Result : {0} => Time : {1}"
                                                            , machineTmp.CompletedLogging.ToString()
                                                            , DateTime.Now));

                    var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST4LoggingApp", machineTmp.CompletedLogging);
                    if (reactResult)
                    {
                        WriteLog("KeepLoggingST4.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST4LoggingApp"
                                                            , DateTime.Now
                                                            , "TRUE"));
                    }
                    else
                    {
                        WriteLog("KeepLoggingST4.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST4LoggingApp"
                                                            , DateTime.Now
                                                            , "FALSE"));
                    }

                    _view.machine5 = machineTmp;
                }
                #endregion

                #region Station5
                if (machine.Id == 6)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine6;
                    _view.machine6.MessageResult = string.Empty;

                    try
                    {
                        WriteLog("KeepLoggingST5_1.txt", String.Format("<<=================== Start time : {0} ===================>>", DateTime.Now));
                        keepLog = KeepLogForMachine6(r, machine, machineTags);
                        if (keepLog)
                            machineTmp.CompletedLogging = 1;
                        else
                            machineTmp.CompletedLogging = 3;
                    }
                    catch (Exception ex)
                    {
                        _view.machine6.MessageResult = ex.Message;
                        machineTmp.CompletedLogging = 3;
                        WriteLog("KeepLoggingST5_1.txt", "<<=================== Begin Error Exception ===================>>");
                        WriteLog("KeepLoggingST5_1.txt", ex.Message);
                        WriteLog("KeepLoggingST5_1.txt", "<<=================== End Error Exception ===================>>");
                    }

                    WriteLog("KeepLoggingST5_1.txt", String.Format("LoggingApp Result : {0} => Time : {1}"
                                                            , machineTmp.CompletedLogging.ToString()
                                                            , DateTime.Now));
                    var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST5_1LoggingApp", machineTmp.CompletedLogging);
                    WriteLog("KeepLoggingST5_1.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST5_1LoggingApp"
                                                            , DateTime.Now
                                                            , reactResult.ToString()));
                    _view.machine6 = machineTmp;
                }

                if (machine.Id == 7)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine7;
                    _view.machine7.MessageResult = string.Empty;

                    try
                    {
                        WriteLog("KeepLoggingST5_2.txt", String.Format("<<=================== Start time : {0} ===================>>", DateTime.Now));
                        keepLog = KeepLogForMachine7(r, machine, machineTags);
                        if (keepLog)
                            machineTmp.CompletedLogging = 1;
                        else
                            machineTmp.CompletedLogging = 3;
                    }
                    catch (Exception ex)
                    {
                        _view.machine7.MessageResult = ex.Message;
                        machineTmp.CompletedLogging = 3;
                        WriteLog("KeepLoggingST5_2.txt", "<<=================== Begin Error Exception ===================>>");
                        WriteLog("KeepLoggingST5_2.txt", ex.Message);
                        WriteLog("KeepLoggingST5_2.txt", "<<=================== End Error Exception ===================>>");
                    }

                    WriteLog("KeepLoggingST5_2.txt", String.Format("LoggingApp Result : {0} => Time : {1}"
                                                    , machineTmp.CompletedLogging.ToString()
                                                    , DateTime.Now));
                    var reactResult = ReactCompleteLog(_view.tagMainBlock + "ST5_2LoggingApp", machineTmp.CompletedLogging);
                    WriteLog("KeepLoggingST5_2.txt", String.Format("Write PLC Tag : {0} : [{2}] => Time : {1}"
                                                            , "ST5_2LoggingApp"
                                                            , DateTime.Now
                                                            , reactResult.ToString()));
                    _view.machine7 = machineTmp;
                }
                #endregion
            }
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

        private bool KeepLogForMachine1(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
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
            trace.StationId = m.StationId;
            trace.MachineId = m.Id;
            trace.ProductionDate = DateTime.Now;

            #region Validate and Default existing log by Item Code.
            var tagName = _view.tagMainBlock + "ST1Code";
            var itemCode = r.Where(x => x.ItemName == tagName).FirstOrDefault().Value.ToString();
            var loggings = _serviceTraceLog.GetListByItemCode(itemCode)
                                            .Where(x =>
                                                    x.MachineId == m.Id
                                                    //&& x.CreationDate.Date == DateTime.Now.Date
                                                    //&& !x.FinishFlag
                                                    ).ToList();

            if (loggings.Count > 0)
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
                        trace.FinalResult = Convert.ToInt32(item.Value);

                    if (item.ItemName == _view.tagMainBlock + "ST1ModelRunning")
                        trace.ModelRunning = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST1RepairTime")
                        trace.RepairTime = Convert.ToInt32(item.Value);
                }
            }
            #endregion

            #region Keep Part Assemblies
            trace.PartAssemblies = new List<PartAssemblyModel>();
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
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[0]")
                    {
                        part.LineNumber = 1;
                        part.PartName = "UPR Actuator P/N";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[1]")
                    {
                        part.LineNumber = 2;
                        part.PartName = "UPR Actuator S/N";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[2]")
                    {
                        part.LineNumber = 3;
                        part.PartName = "UPR Frame";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[3]")
                    {
                        part.LineNumber = 4;
                        part.PartName = "UPR vane set LH";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[4]")
                    {
                        part.LineNumber = 5;
                        part.PartName = "UPR vane set RH";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[5]")
                    {
                        part.LineNumber = 6;
                        part.PartName = "Pull rod";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[6]")
                    {
                        part.LineNumber = 7;
                        part.PartName = "Lever";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[7]")
                    {
                        part.LineNumber = 8;
                        part.PartName = "Space";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[8]")
                    {
                        part.LineNumber = 9;
                        part.PartName = "Tether";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[9]")
                    {
                        part.LineNumber = 10;
                        part.PartName = "UPR Actuator BOX S/N ";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[10]")
                    {
                        part.LineNumber = 11;
                        part.PartName = "UPR Van set LH BOX S/N";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[11]")
                    {
                        part.LineNumber = 12;
                        part.PartName = "UPR Van set RH BOX S/N";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[12]")
                    {
                        part.LineNumber = 13;
                        part.PartName = "Tether BOX S/N";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[13]")
                    {
                        part.LineNumber = 14;
                        part.PartName = "UPR Frame BOX S/N";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[14]")
                    {
                        part.LineNumber = 15;
                        part.PartName = "PullRod BOX S/N";
                        part.SerialNumber = item.Value.ToString();
                    }
                    if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[15]")
                    {
                        part.LineNumber = 16;
                        part.PartName = "Space BOX S/N";
                        part.SerialNumber = item.Value.ToString();
                    }

                    trace.PartAssemblies.Add(part);
                }
            }
            #endregion

            #region Keep Tightening
            List<TighteningRepairModel> tRepairs = new List<TighteningRepairModel>();
            TighteningRepairModel tRepair = new TighteningRepairModel();
            TighteningResultModel tmp = new TighteningResultModel();

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
                    cam.CameraName = "Lever Assy L";
                    cam.TestResult = item.Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[9]")
                {
                    cam.CameraName = "Lever Assy R";
                    cam.TestResult = item.Value.ToString();
                }

                trace.CameraResults.Add(cam);
            }
            #endregion

            #region Insert/Update Logging Detail
            if (invalid)
            {
                var mac = _view.machine1;
                mac.MessageResult = errMsg;
                mac.CompletedLogging = 3;
                _view.machine1 = mac;
                result = false;
            }
            else
            {
                TraceabilityLogModel logResult = new TraceabilityLogModel();
                if (trace.FinalResult != 0)
                    trace.FinishFlag = true;

                if (trace.Id == 0)
                {
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
                }
                else
                {
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

        private bool KeepLogForMachine2(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
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
            trace.StationId = m.StationId;
            trace.MachineId = m.Id;
            trace.ProductionDate = DateTime.Now;

            #region  Validate and Default existing log by Item Code.
            var tagName = _view.tagMainBlock + "ST2Code";
            var itemCode = r.Where(x => x.ItemName == tagName).FirstOrDefault().Value.ToString();
            var loggings = _serviceTraceLog.GetListByItemCode(itemCode)
                                            .Where(x =>
                                                   x.MachineId == m.Id
                                                   //&& x.CreationDate.Date == DateTime.Now.Date
                                                   //&& !x.FinishFlag
                                                   ).ToList();

            if (loggings.Count > 0)
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
                        trace.FinalResult = Convert.ToInt32(item.Value);

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
                var mac = _view.machine2;
                mac.MessageResult = errMsg;
                mac.CompletedLogging = 3;
                _view.machine2 = mac;
                result = false;
            }
            else
            {
                if (trace.FinalResult != 0)
                    trace.FinishFlag = true;

                if (trace.Id == 0)
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
                }
                else
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
                }
            }
            #endregion
            return result;
        }

        private bool KeepLogForMachine3(IEnumerable<ItemValueResult> r, MachineModel m)
        {
            bool result = true;
            bool invalid = false;
            string errMsg = string.Empty;
            string tmpMsg = string.Empty;

            TraceabilityLogModel trace = new TraceabilityLogModel();
            trace.StationId = m.StationId;
            trace.MachineId = m.Id;
            trace.Description = "UPPER AGS FRAM";
            trace.ProductionDate = DateTime.Now;

            foreach (var item in r)
            {
                tmpMsg = string.Empty;
                if (InvalidDataTag(item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }

                if (!invalid)
                {
                    if (item.ItemName == _view.tagMainBlock + "ST3_1Code")
                        trace.ItemCode = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST3_1TestResult[0]")
                        trace.Attribute1 = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST3_1Final_Judgment")
                        trace.FinalResult = Convert.ToInt32(item.Value);

                    if (item.ItemName == _view.tagMainBlock + "ST3_1RepairTime")
                        trace.RepairTime = Convert.ToInt32(item.Value);
                }
            }

            if (invalid)
            {
                var mac = _view.machine3;
                mac.MessageResult = errMsg;
                mac.CompletedLogging = 3;
                _view.machine3 = mac;
                result = false;
            }
            else
            {
                try
                {
                    _serviceTraceLog.Create(trace);
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        private bool KeepLogForMachine4(IEnumerable<ItemValueResult> r, MachineModel m)
        {
            bool result = true;
            bool invalid = false;
            string errMsg = string.Empty;
            string tmpMsg = string.Empty;

            TraceabilityLogModel trace = new TraceabilityLogModel();
            trace.StationId = m.StationId;
            trace.MachineId = m.Id;
            trace.ProductionDate = DateTime.Now;
            trace.Description = "LOWER AGS FRAM";

            foreach (var item in r)
            {
                tmpMsg = string.Empty;
                if (InvalidDataTag(item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }

                if (!invalid)
                {
                    if (item.ItemName == _view.tagMainBlock + "ST3_2Code")
                        trace.ItemCode = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST3_2TestResult[0]")
                        trace.Attribute1 = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST3_2Final_Judgment")
                        trace.FinalResult = Convert.ToInt32(item.Value);

                    if (item.ItemName == _view.tagMainBlock + "ST3_2RepairTime")
                        trace.RepairTime = Convert.ToInt32(item.Value);
                }
            }

            if (invalid)
            {
                var mac = _view.machine4;
                mac.MessageResult = errMsg;
                mac.CompletedLogging = 3;
                _view.machine4 = mac;
                result = false;
            }
            else
            {
                try
                {
                    _serviceTraceLog.Create(trace);
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        private bool KeepLogForMachine5(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
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

            trace.StationId = m.StationId;
            trace.MachineId = m.Id;
            trace.ProductionDate = DateTime.Now;

            #region Tightening Keep position #001
            //Tightening Keep position #001
            var tagName = _view.tagMainBlock + "ST4Code";
            var itemCode = r.Where(x => x.ItemName == tagName).FirstOrDefault().Value.ToString();
            WriteLog("KeepLoggingST4.txt", String.Format("Item Code = {0}", itemCode));
            var loggings = _serviceTraceLog.GetListByItemCode(itemCode)
                                            .Where(x =>
                                                   x.MachineId == m.Id
                                                   //&& x.CreationDate.Date == DateTime.Now.Date
                                                   //&& !x.FinishFlag
                                                   ).ToList();

            if (loggings.Count > 0)
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
                        trace.FinalResult = Convert.ToInt32(item.Value);

                    if (item.ItemName == _view.tagMainBlock + "ST4ModelRunning")
                        trace.ModelRunning = item.Value.ToString();
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

                    if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[2]")
                    {
                        part.LineNumber = 3;
                        part.PartName = "LWR Frame";
                        part.SerialNumber = item.Value.ToString();
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

                    trace.PartAssemblies.Add(part);
                }
            }

            //Keep Tightening
            int i = 1;
            List<TighteningRepairModel> tRepairs = new List<TighteningRepairModel>();
            TighteningRepairModel tRepair = new TighteningRepairModel();
            TighteningResultModel tmp = new TighteningResultModel();

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

            if (invalid)
            {
                var mac = _view.machine5;
                mac.MessageResult = errMsg;
                mac.CompletedLogging = 3;
                _view.machine5 = mac;
                result = false;
            }
            else
            {
                if (trace.FinalResult != 0)
                    trace.FinishFlag = true;

                if (trace.Id == 0)
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
                }
                else
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
                }
            }

            return result;
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

        private bool KeepLogForMachine6(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
        {
            bool result = true;
            bool invalid = false;
            string errMsg = string.Empty;
            string tmpMsg = string.Empty;

            TraceabilityLogModel trace = new TraceabilityLogModel();

            //Default Station and Machine
            trace.StationId = m.StationId;
            trace.MachineId = m.Id;
            trace.Description = "Upper  EOL laser marking and probe sensor check";

            #region Validate and Default existing log by Item Code.
            var tagName = _view.tagMainBlock + "ST5_1Code";
            var itemCode = r.Where(x => x.ItemName == tagName).FirstOrDefault().Value.ToString();
            var loggings = _serviceTraceLog.GetListByItemCode(itemCode)
                                            .Where(x =>
                                                    x.MachineId == m.Id
                                                    //&& x.CreationDate.Date == DateTime.Now.Date
                                                    //&& !x.FinishFlag
                                                    ).ToList();

            if (loggings.Count > 0)
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
                }
            }
            #endregion

            #region Keep Master Logging
            foreach (var item in r)
            {
                if (!invalid)
                {
                    if (item.ItemName == _view.tagMainBlock + "ST5_1Code")
                        trace.ItemCode = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_1_Product_Serial_No.")
                        trace.PartSerialNumber = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[0]")
                        trace.Actuator = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[1]")
                    {
                        string strDate = item.Value.ToString();

                        if (strDate.Length > 2)
                        {
                            trace.ProductionDate = GetProductionDate(strDate);
                        }
                        else
                        {
                            invalid = true;
                            tmpMsg = "Production Date was not recognized as a valid DateTime.";
                            if (!string.IsNullOrEmpty(errMsg))
                                errMsg += Environment.NewLine;

                            errMsg += tmpMsg;
                        }
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[2]")
                        trace.SwNumber = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[3]")
                        trace.LineErrorCounter = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[4]")
                        trace.CurrentMaximum = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[5]")
                        trace.OpenAngle = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[6]")
                        trace.Attribute1 = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[7]")
                        trace.Attribute2 = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_1Final_Judgment")
                        trace.FinalResult = Convert.ToInt32(item.Value);

                    if (item.ItemName == _view.tagMainBlock + "ST5_1RepairTime")
                        trace.RepairTime = Convert.ToInt32(item.Value);
                }
            }
            #endregion

            #region Insert/Update Logging Detail
            if (invalid)
            {
                var mac = _view.machine6;
                mac.MessageResult = errMsg;
                mac.CompletedLogging = 3;
                _view.machine6 = mac;
                result = false;
            }
            else
            {
                if (trace.FinalResult != 0)
                    trace.FinishFlag = true;

                string tartgetFile;
                string errMessageFile;

                if (trace.Id == 0)
                {
                    _serviceTraceLog.Create(trace);
                    KeeppingFile(trace.ItemCode, out tartgetFile, out errMessageFile);
                }
                else
                {
                    _serviceTraceLog.Update(trace);
                    KeeppingFile(trace.ItemCode, out tartgetFile, out errMessageFile);
                }
            }
            #endregion

            return result;
        }

        private bool KeepLogForMachine7(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
        {
            bool result = true;
            bool invalid = false;
            string errMsg = string.Empty;
            string tmpMsg = string.Empty;

            TraceabilityLogModel trace = new TraceabilityLogModel();

            //Default Station and Machine
            trace.StationId = m.StationId;
            trace.MachineId = m.Id;
            trace.Description = "Lower EOL laser marking and probe sensor check";

            #region Validate and Default existing log by Item Code.
            var tagName = _view.tagMainBlock + "ST5_2Code";
            var itemCode = r.Where(x => x.ItemName == tagName).FirstOrDefault().Value.ToString();
            var loggings = _serviceTraceLog.GetListByItemCode(itemCode)
                                            .Where(x =>
                                                    x.MachineId == m.Id
                                                    //&& x.CreationDate.Date == DateTime.Now.Date
                                                    //&& !x.FinishFlag
                                                    ).ToList();

            if (loggings.Count > 0)
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
                }
            }
            #endregion

            #region Keep Master Logging
            foreach (var item in r)
            {
                if (!invalid)
                {
                    if (item.ItemName == _view.tagMainBlock + "ST5_2Code")
                        trace.ItemCode = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_2_Product_Serial_No.")
                        trace.PartSerialNumber = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[0]")
                        trace.Actuator = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[1]")
                    {
                        string strDate = item.Value.ToString();

                        if (strDate.Length > 2)
                        {
                            trace.ProductionDate = GetProductionDate(strDate);
                        }
                        else
                        {
                            invalid = true;
                            tmpMsg = "Production Date was not recognized as a valid DateTime.";
                            if (!string.IsNullOrEmpty(errMsg))
                                errMsg += Environment.NewLine;

                            errMsg += tmpMsg;
                        }
                    }

                    if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[2]")
                        trace.SwNumber = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[3]")
                        trace.LineErrorCounter = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[4]")
                        trace.CurrentMaximum = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[5]")
                        trace.OpenAngle = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[6]")
                        trace.Attribute1 = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[7]")
                        trace.Attribute2 = item.Value.ToString();

                    if (item.ItemName == _view.tagMainBlock + "ST5_2Final_Judgment")
                        trace.FinalResult = Convert.ToInt32(item.Value);

                    if (item.ItemName == _view.tagMainBlock + "ST5_2RepairTime")
                        trace.RepairTime = Convert.ToInt32(item.Value);
                }
            }
            #endregion

            #region Insert/Update Logging Detail
            if (invalid)
            {
                var mac = _view.machine7;
                mac.MessageResult = errMsg;
                mac.CompletedLogging = 3;
                _view.machine7 = mac;
                result = false;
            }
            else
            {
                string tartgetFile;
                string errMessageFile;

                if (trace.FinalResult != 0)
                    trace.FinishFlag = true;

                if (trace.Id == 0)
                {
                    _serviceTraceLog.Create(trace);
                    KeeppingFile(trace.ItemCode, out tartgetFile, out errMessageFile);
                }
                else
                {
                    _serviceTraceLog.Update(trace);
                    KeeppingFile(trace.ItemCode, out tartgetFile, out errMessageFile);
                }
            }
            #endregion

            return result;
        }

        private DateTime GetProductionDate(string strDate)
        {
            DateTime prodDate = DateTime.Now;
            try
            {
                DateTime resultYear;
                int year = Convert.ToInt32(strDate.Substring(0, 2));
                int day = Convert.ToInt32(strDate.Substring(2, 3));
                //TimeSpan ts = new TimeSpan();

                bool canParse = DateTime.TryParseExact(year.ToString(),
                    "yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out resultYear);

                if (canParse)
                    prodDate = resultYear;

                prodDate = new DateTime(prodDate.Year, 1, 1).AddDays(day - 1);
                prodDate = prodDate.Add(DateTime.Now.TimeOfDay);
            }
            catch
            {
                prodDate = DateTime.Now;
            }

            return prodDate;
        }

        private void Disconnect_Click(object sender, EventArgs e)
        {
            _view.connectedPlc = !Disconnect();
        }

        private void MakeReady(object sender, EventArgs e)
        {
            if (_view.systemReady)
            {
                if (WriteWord(_view.tagTraceabilityReady, 0))
                    _view.systemReady = false;
            }
            else
            {
                if (WriteWord(_view.tagTraceabilityReady, 1))
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
            TimeSpan ts = TimeSpan.FromMilliseconds(5000);
            //if (!t.Wait(ts))
            //    Console.WriteLine("The timeout interval elapsed.");

            if (!t.Wait(ts))
            {
                _view.connectedPlc = false;
                _view.systemReady = false;
                Disconnect();
                _view.strConnectionMessage = "Inter Lock failed!";
                //MessageBox.Show("Connection failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Application.Exit();
            }
            else
            {
                _view.strConnectionMessage = "Inter Lock completed.";
            }
        }

        private bool ReactCompleteLog(string tagName, int val = 1)
        {
            return WriteWord(tagName, val);
        }

        private bool ReactCompleteLog1(string tagName, int val = 1)
        {
            bool result = false;

            Task t = Task.Run(() => {
                result = WriteWord(tagName, val);
            });
            TimeSpan ts = TimeSpan.FromMilliseconds(4000);

            if (!t.Wait(ts))
            {
                result = false;
            }

            return result;
        }

        private bool ReactDataTag(string tagName, string val = "")
        {
            return WriteString(tagName, val);
        }

        private bool ReactDataTag1(string tagName, string val = "")
        {
            bool result = false;

            Task t = Task.Run(() => {
                result = WriteString(tagName, val);
            });
            TimeSpan ts = TimeSpan.FromMilliseconds(4000);

            if (!t.Wait(ts))
            {
                result = false;
            }

            return result;
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

        private bool Disconnect()
        {
            if (_view.daServer != null && _view.daServer.IsConnected)
                _view.daServer.Disconnect();

            return true;
        }

        private void FormLoad(object sender, EventArgs e)
        {

            _view.serverUrl = ConfigurationManager.AppSettings["DefaultUrl"].ToString();
            _view.tagMainBlock = ConfigurationManager.AppSettings["MainBlock"].ToString();

            var machines = _serviceMachine.GetAll();
            int i = 1;
            foreach (var mac in machines.ToList().OrderBy(o => o.Id))
            {
                if (i == 1)
                    _view.machine1 = mac;
                if (i == 2)
                    _view.machine2 = mac;
                if (i == 3)
                    _view.machine3 = mac;
                if (i == 4)
                    _view.machine4 = mac;
                if (i == 5)
                    _view.machine5 = mac;
                if (i == 6)
                    _view.machine6 = mac;
                if (i == 7)
                    _view.machine7 = mac;

                i++;
            }

            var result = _servicePLCTag.GetAll();
            _view.plcTags = result.ToList();

            if (_view.plcTags.Count > 0)
            {
                string clockTag = _view.plcTags.Where(x => x.TypeCode == "INTER_LOCK").FirstOrDefault().PlcTag;
                string readyTag = _view.plcTags.Where(x => x.TypeCode == "SYSTEM_READY").FirstOrDefault().PlcTag;
                _view.tagClockReady = _view.tagMainBlock + clockTag;
                _view.tagTraceabilityReady = _view.tagMainBlock + readyTag;
            }

        }

        private bool Connect(string serverName)
        {
            /* When the factory creates an HDA server, it passes along 2 parameters:
             *    SerializationInfo info
             *    StreamingContext context
             *
             * The Factory class casts the COM object (pointing to the HDA server) to the IServer interface.
             * All calls to the interface or proxied to the COM object.
             */
            Opc.URL url = new Opc.URL(serverName);
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
                    //MessageBox.Show(ex.Message, "Connect OPC Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                //Console.WriteLine(String.Format("Connect to server {0}", serverName));

                //3rd Create a group if items            
                _view.groupStateRead = new SubscriptionState();
                _view.groupStateRead.Name = "InterLockGroup";
                _view.groupStateRead.UpdateRate = 500;// this isthe time between every reads from OPC server
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

                _view.groupStateWrite = new SubscriptionState();
                _view.groupStateWrite.Name = "Group Write";
                _view.groupStateWrite.Active = false;//not needed to read if you want to write only
                _view.groupWrite = (Subscription)_view.daServer.CreateSubscription(_view.groupStateWrite);

                return true;
            }
            catch (Opc.ConnectFailedException opcConnExc)
            {
                MessageBox.Show(String.Format("Could not connect to server {0}{1}{2}"
                                            , serverName
                                            , Environment.NewLine
                                            , opcConnExc.ToString())
                                            , "Connection failed!"
                                            , MessageBoxButtons.OK
                                            , MessageBoxIcon.Error);
                return false;
            }
        }

        private void LoadCurrentValue(Subscription groupRead)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                var currentResult = _view.groupRead.Read(_view.groupRead.Items).ToList();

                //Machine 1
                var mac1 = _view.machine1;
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

                _view.machine1 = mac1;

                //Machine 2
                var mac2 = _view.machine2;
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

                _view.machine2 = mac2;

                //Machine 3
                var mac3 = _view.machine3;
                var onlineTag3 = _view.tagMainBlock + "ST3_1StatusMc";
                mac3.OnlineFlag = Convert.ToInt32(currentResult.Where(x => x.ItemName == onlineTag3).FirstOrDefault().Value);

                var reqLoggingTag3 = _view.tagMainBlock + "ST3_1ReqLogging";
                mac3.RequestLogging = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqLoggingTag3).FirstOrDefault().Value);

                var completeLoggingTag3 = _view.tagMainBlock + "ST3_1LoggingApp";
                mac3.CompletedLogging = Convert.ToInt32(currentResult.Where(x => x.ItemName == completeLoggingTag3).FirstOrDefault().Value);

                var reqVerifyTag3 = _view.tagMainBlock + "ST3_1ReqChkCodeVerify";
                mac3.RequestVerifyCode = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqVerifyTag3).FirstOrDefault().Value);

                var verifyResultTag3 = _view.tagMainBlock + "ST3_1CodeVerifyResult";
                mac3.CodeVerifyResult = Convert.ToInt32(currentResult.Where(x => x.ItemName == verifyResultTag3).FirstOrDefault().Value);

                _view.machine3 = mac3;

                //Machine 4
                var mac4 = _view.machine4;
                var onlineTag4 = _view.tagMainBlock + "ST3_2StatusMc";
                mac4.OnlineFlag = Convert.ToInt32(currentResult.Where(x => x.ItemName == onlineTag4).FirstOrDefault().Value);

                var reqLoggingTag4 = _view.tagMainBlock + "ST3_2ReqLogging";
                mac4.RequestLogging = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqLoggingTag4).FirstOrDefault().Value);

                var completeLoggingTag4 = _view.tagMainBlock + "ST3_2LoggingApp";
                mac4.CompletedLogging = Convert.ToInt32(currentResult.Where(x => x.ItemName == completeLoggingTag4).FirstOrDefault().Value);

                var reqVerifyTag4 = _view.tagMainBlock + "ST3_2ReqChkCodeVerify";
                mac4.RequestVerifyCode = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqVerifyTag4).FirstOrDefault().Value);

                var verifyResultTag4 = _view.tagMainBlock + "ST3_2CodeVerifyResult";
                mac4.CodeVerifyResult = Convert.ToInt32(currentResult.Where(x => x.ItemName == verifyResultTag4).FirstOrDefault().Value);

                _view.machine4 = mac4;

                //Machine 5
                var mac5 = _view.machine5;
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

                _view.machine5 = mac5;

                //Machine 6
                var mac6 = _view.machine6;
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

                _view.machine6 = mac6;

                //Machine 7
                var mac7 = _view.machine7;
                var onlineTag7 = _view.tagMainBlock + "ST5_2StatusMc";
                mac7.OnlineFlag = Convert.ToInt32(currentResult.Where(x => x.ItemName == onlineTag7).FirstOrDefault().Value);

                var reqLoggingTag7 = _view.tagMainBlock + "ST5_2ReqLogging";
                mac7.RequestLogging = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqLoggingTag7).FirstOrDefault().Value);

                var completeLoggingTag7 = _view.tagMainBlock + "ST5_2LoggingApp";
                mac7.CompletedLogging = Convert.ToInt32(currentResult.Where(x => x.ItemName == completeLoggingTag7).FirstOrDefault().Value);

                var reqVerifyTag7 = _view.tagMainBlock + "ST5_2ReqChkCodeVerify";
                mac7.RequestVerifyCode = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqVerifyTag7).FirstOrDefault().Value);

                var verifyResultTag7 = _view.tagMainBlock + "ST5_2CodeVerifyResult";
                mac7.CodeVerifyResult = Convert.ToInt32(currentResult.Where(x => x.ItemName == verifyResultTag7).FirstOrDefault().Value);

                _view.machine7 = mac7;

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor.Current = Cursors.Default;
                return;
            }
        }

        private bool WriteWordInterLock(string tag, int value)
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

        private bool WriteWord(string tag, int value)
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

        public string RemoveSpecialCharacters(string str)
        {
            string spath = Regex.Replace(str, "[^a-zA-Z0-9_.:]+", "\\", RegexOptions.Compiled);
            spath = spath.Replace("\\.", ".");
            spath = spath.Replace("\\D", "D");
            spath = spath.Replace("\\_", "_");
            return spath.Replace("\\E", "E");
        }

        private bool InvalidDataTag(string value, string tagName, out string tmpMsg)
        {
            bool result = false;
            string returnMsg = string.Empty;
            var tags = _view.plcTags;
            var tag = tags.Where(x => _view.tagMainBlock + x.PlcTag == tagName).FirstOrDefault();

            if (tag != null)
            {
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
