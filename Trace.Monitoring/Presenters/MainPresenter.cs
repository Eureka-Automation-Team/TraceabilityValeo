using Opc.Da;
using System;
using System.Collections.Generic;
using System.Configuration;
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

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString()).Result
                                               .Where(x => x.MachineId == 1);

                if(loggings.Count() > 0)
                {
                    var maxRework = loggings.OrderBy(o => o.CreationDate).FirstOrDefault().RepairTime;
                    if (loggings.Count() >= maxRework)
                    {
                        _machine.CodeVerifyResult = 3;  //Over rework time
                    }
                    else
                    {
                        var firstResult = loggings.FirstOrDefault();
                        _machine.CodeVerifyResult = firstResult.FinalResult == 1 ? 1 : 2;
                    }
                }
                else
                {
                    _machine.CodeVerifyResult = 4; //Data not found
                }

                _view.machine1 = _machine;
                ReactCompleteLog(_view.tagMainBlock + "ST1CodeVerifyResult", _machine.CodeVerifyResult);
            }

            //machine 2
            if (_machine.Id == 2)
            {
                var tagName = _view.tagMainBlock + "ST2CodeVerify";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString()).Result
                                               .Where(x => x.MachineId == 2);

                if (loggings.Count() > 0)
                {
                    var maxRework = loggings.OrderBy(o => o.CreationDate).FirstOrDefault().RepairTime;
                    if (loggings.Count() >= maxRework)
                    {
                        _machine.CodeVerifyResult = 3;  //Over rework time
                    }
                    else
                    {
                        var firstResult = loggings.FirstOrDefault();
                        _machine.CodeVerifyResult = firstResult.FinalResult == 1 ? 1 : 2;
                    }
                }
                else
                {
                    _machine.CodeVerifyResult = 4; //Data not found
                }

                _view.machine2 = _machine;
                ReactCompleteLog(_view.tagMainBlock + "ST2CodeVerifyResult", _machine.CodeVerifyResult);
            }

            //machine 3
            if (_machine.Id == 3)
            {
                var tagName = _view.tagMainBlock + "ST3_1CodeVerify";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString()).Result
                                               .Where(x => x.MachineId == 3);

                if (loggings.Count() > 0)
                {
                    var maxRework = loggings.OrderBy(o => o.CreationDate).FirstOrDefault().RepairTime;
                    if (loggings.Count() >= maxRework)
                    {
                        _machine.CodeVerifyResult = 3;  //Over rework time
                    }
                    else
                    {
                        var firstResult = loggings.FirstOrDefault();
                        _machine.CodeVerifyResult = firstResult.FinalResult == 1 ? 1 : 2;
                    }
                }
                else
                {
                    _machine.CodeVerifyResult = 4; //Data not found
                }

                _view.machine3 = _machine;
                ReactCompleteLog(_view.tagMainBlock + "ST3_1CodeVerifyResult", _machine.CodeVerifyResult);
            }

            //machine 4
            if (_machine.Id == 4)
            {
                var tagName = _view.tagMainBlock + "ST3_2CodeVerify";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString()).Result
                                               .Where(x => x.MachineId == 4);

                if (loggings.Count() > 0)
                {
                    var maxRework = loggings.OrderBy(o => o.CreationDate).FirstOrDefault().RepairTime;
                    if (loggings.Count() >= maxRework)
                    {
                        _machine.CodeVerifyResult = 3;  //Over rework time
                    }
                    else
                    {
                        var firstResult = loggings.FirstOrDefault();
                        _machine.CodeVerifyResult = firstResult.FinalResult == 1 ? 1 : 2;
                    }
                }
                else
                {
                    _machine.CodeVerifyResult = 4; //Data not found
                }

                _view.machine4 = _machine;
                ReactCompleteLog(_view.tagMainBlock + "ST3_2CodeVerifyResult", _machine.CodeVerifyResult);
            }

            //machine 5
            if (_machine.Id == 5)
            {
                var tagName = _view.tagMainBlock + "ST4CodeVerify";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString()).Result
                                               .Where(x => x.MachineId == 5);

                if (loggings.Count() > 0)
                {
                    var maxRework = loggings.OrderBy(o => o.CreationDate).FirstOrDefault().RepairTime;
                    if (loggings.Count() >= maxRework)
                    {
                        _machine.CodeVerifyResult = 3;  //Over rework time
                    }
                    else
                    {
                        var firstResult = loggings.FirstOrDefault();
                        _machine.CodeVerifyResult = firstResult.FinalResult == 1 ? 1 : 2;
                    }
                }
                else
                {
                    _machine.CodeVerifyResult = 4; //Data not found
                }

                _view.machine5 = _machine;
                ReactCompleteLog(_view.tagMainBlock + "ST4CodeVerifyResult", _machine.CodeVerifyResult);
            }

            //machine 6
            if (_machine.Id == 6)
            {
                var tagName = _view.tagMainBlock + "ST5_1CodeVerify";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString()).Result
                                               .Where(x => x.MachineId == 6);

                if (loggings.Count() > 0)
                {
                    var maxRework = loggings.OrderBy(o => o.CreationDate).FirstOrDefault().RepairTime;
                    if (loggings.Count() >= maxRework)
                    {
                        _machine.CodeVerifyResult = 3;  //Over rework time
                    }
                    else
                    {
                        var firstResult = loggings.FirstOrDefault();
                        _machine.CodeVerifyResult = firstResult.FinalResult == 1 ? 1 : 2;
                    }
                }
                else
                {
                    _machine.CodeVerifyResult = 4; //Data not found
                }

                _view.machine6 = _machine;
                ReactCompleteLog(_view.tagMainBlock + "ST5_1CodeVerifyResult", _machine.CodeVerifyResult);
            }

            //machine 7
            if (_machine.Id == 7)
            {
                var tagName = _view.tagMainBlock + "ST5_2CodeVerify";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString()).Result
                                               .Where(x => x.MachineId == 7);

                if (loggings.Count() > 0)
                {
                    var maxRework = loggings.OrderBy(o => o.CreationDate).FirstOrDefault().RepairTime;
                    if (loggings.Count() >= maxRework)
                    {
                        _machine.CodeVerifyResult = 3;  //Over rework time
                    }
                    else
                    {
                        var firstResult = loggings.FirstOrDefault();
                        _machine.CodeVerifyResult = firstResult.FinalResult == 1 ? 1 : 2;
                    }
                }
                else
                {
                    _machine.CodeVerifyResult = 4; //Data not found
                }

                _view.machine7 = _machine;
                ReactCompleteLog(_view.tagMainBlock + "ST5_2CodeVerifyResult", _machine.CodeVerifyResult);
            }
        }

        private async void CompleteAction(object sender, EventArgs e)
        {
            Button but = sender as Button;
            var result = _view.groupRead.Read(_view.groupRead.Items).ToList();

            if (but.Name == "butCompletedLogging1")
            {
                var tagName = _view.tagMainBlock + "ST1Code";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString());
                if (loggings != null) {
                    ReactCompleteLog(_view.tagMainBlock + "ST1LoggingApp", 1);
                }
                else
                {
                    MachineModel machine = await _serviceMachine.GetByID(1);

                    var machineTags = _servicePLCTag.GetAll().Result.ToList().Where(x => x.MachineId == machine.Id);
                    var tags = (from tag in machineTags
                                where tag.MachineId == machine.Id
                                select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                    var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                    bool keepLog = await KeepLogForMachine1(r, machine, machineTags);
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
                if (loggings != null)
                {
                    ReactCompleteLog(_view.tagMainBlock + "ST2LoggingApp", 1);
                }
                else
                {
                    MachineModel machine = await _serviceMachine.GetByID(2);

                    var machineTags = _servicePLCTag.GetAll().Result.ToList().Where(x => x.MachineId == machine.Id);
                    var tags = (from tag in machineTags
                                where tag.MachineId == machine.Id
                                select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                    var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                    bool keepLog = await KeepLogForMachine2(r, machine, machineTags);
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
                if (loggings != null)
                {
                    ReactCompleteLog(_view.tagMainBlock + "ST3_1LoggingApp", 1);
                }
                else
                {
                    MachineModel machine = await _serviceMachine.GetByID(3);

                    var machineTags = _servicePLCTag.GetAll().Result.ToList().Where(x => x.MachineId == machine.Id);
                    var tags = (from tag in machineTags
                                where tag.MachineId == machine.Id
                                select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                    var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                    bool keepLog = await KeepLogForMachine3(r, machine);
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
                if (loggings != null)
                {
                    ReactCompleteLog(_view.tagMainBlock + "ST3_2LoggingApp", 1);
                }
                else
                {
                    MachineModel machine = await _serviceMachine.GetByID(4);

                    var machineTags = _servicePLCTag.GetAll().Result.ToList().Where(x => x.MachineId == machine.Id);
                    var tags = (from tag in machineTags
                                where tag.MachineId == machine.Id
                                select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                    var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                    bool keepLog = await KeepLogForMachine4(r, machine);
                    if (keepLog)
                    {
                        ReactCompleteLog(_view.tagMainBlock + "ST3_2LoggingApp", 1);
                    }
                }
            }

            if (but.Name == "butCompletedLogging5")
            {
                var tagName = _view.tagMainBlock + "ST4Code";
                var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                var loggings = _serviceTraceLog.GetListByItemCode(value.ToString());
                if (loggings != null)
                {
                    ReactCompleteLog(_view.tagMainBlock + "ST4LoggingApp", 1);
                }
                else
                {
                    MachineModel machine = await _serviceMachine.GetByID(5);

                    var machineTags = _servicePLCTag.GetAll().Result.ToList().Where(x => x.MachineId == machine.Id);
                    var tags = (from tag in machineTags
                                where tag.MachineId == machine.Id
                                select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                    var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                    bool keepLog = await KeepLogForMachine5(r, machine, machineTags);
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
                if (loggings != null)
                {
                    ReactCompleteLog(_view.tagMainBlock + "ST5_1LoggingApp", 1);
                }
                else
                {
                    MachineModel machine = await _serviceMachine.GetByID(6);

                    var machineTags = _servicePLCTag.GetAll().Result.ToList().Where(x => x.MachineId == machine.Id);
                    var tags = (from tag in machineTags
                                where tag.MachineId == machine.Id
                                select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                    var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                    bool keepLog = await KeepLogForMachine6(r, machine, machineTags);
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
                if (loggings != null)
                {
                    ReactCompleteLog(_view.tagMainBlock + "ST5_2LoggingApp", 1);
                }
                else
                {
                    MachineModel machine = await _serviceMachine.GetByID(7);

                    var machineTags = _servicePLCTag.GetAll().Result.ToList().Where(x => x.MachineId == machine.Id);
                    var tags = (from tag in machineTags
                                where tag.MachineId == machine.Id
                                select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                    var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                    bool keepLog = await KeepLogForMachine7(r, machine, machineTags);
                    if (keepLog)
                    {
                        ReactCompleteLog(_view.tagMainBlock + "ST5_2LoggingApp", 1);
                    }
                }
            }
        }

        private async void KeepLogging(object sender, EventArgs e)
        {
            if (_view.systemReady)
            {
                MachineModel machine = (MachineModel)sender;
                ItemValueResult[] subscipt;
                try
                {
                    subscipt = _view.groupRead.Read(_view.groupRead.Items);
                }catch(Opc.ResultIDException ex)
                {
                    _view.connectedPlc = false;
                    _view.systemReady = false;
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = subscipt.ToList();
                var machineTags = _servicePLCTag.GetAll().Result.ToList().Where(x => x.MachineId == machine.Id);
                var tags = (from tag in machineTags
                            where tag.MachineId == machine.Id
                            select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                //Station1
                if (machine.Id == 1)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine1;
                    machineTmp.MessageResult = string.Empty;

                    var tagName = _view.tagMainBlock + "ST1Code";
                    var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                    var loggings = _serviceTraceLog.GetListByItemCode(value.ToString()).Result.Where(x => x.MachineId == machine.Id);

                    if(loggings.Count() > 0)
                    {
                        machineTmp.MessageResult = string.Format("Code :{0} is dupplicated.", value);
                        machineTmp.CompletedLogging = 2;
                    }
                    else
                    {
                        try
                        {
                            keepLog = await KeepLogForMachine1(r, machine, machineTags);
                            if(keepLog)
                                machineTmp.CompletedLogging = 1;
                        }
                        catch (Exception ex)
                        {
                            machineTmp.MessageResult = ex.Message;
                            machineTmp.CompletedLogging = 3;
                        }                        
                    }
                    ReactCompleteLog(_view.tagMainBlock + "ST1LoggingApp", machineTmp.CompletedLogging);
                    _view.machine1 = machineTmp;
                }

                //Station2
                if (machine.Id == 2)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine2;
                    machineTmp.MessageResult = string.Empty;

                    #region Check Dupplicate Item code
                    var tagName = _view.tagMainBlock + "ST2Code";
                    var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                    var loggings = _serviceTraceLog.GetListByItemCode(value.ToString()).Result.Where(x => x.MachineId == machine.Id);

                    if (loggings.Count() > 0)
                    {
                        machineTmp.MessageResult = string.Format("Code :{0} is dupplicated.", value);
                        machineTmp.CompletedLogging = 2;
                    }
                    else
                    {
                        try
                        {
                            keepLog = await KeepLogForMachine2(r, machine, machineTags);
                            machineTmp.CompletedLogging = 1;
                        }
                        catch (Exception ex)
                        {
                            machineTmp.MessageResult = ex.Message;
                            machineTmp.CompletedLogging = 3;
                        }
                    }
                    #endregion
                    ReactCompleteLog(_view.tagMainBlock + "ST2LoggingApp", machineTmp.CompletedLogging);
                    _view.machine2 = machineTmp;
                }

                //Station3
                if (machine.Id == 3)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine3;
                    _view.machine3.MessageResult = string.Empty;

                    #region Check Dupplicate Item code
                    var tagName = _view.tagMainBlock + "ST3_1Code";
                    var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                    var loggings = _serviceTraceLog.GetListByItemCode(value.ToString()).Result.Where(x => x.MachineId == machine.Id);

                    if (loggings.Count() > 0)
                    {
                        machineTmp.MessageResult = string.Format("Code :{0} is dupplicated.", value);
                        machineTmp.CompletedLogging = 2;
                    }
                    else
                    {
                        try
                        {
                            keepLog = await KeepLogForMachine3(r, machine);
                            machineTmp.CompletedLogging = 1;
                        }
                        catch (Exception ex)
                        {
                            _view.machine3.MessageResult = ex.Message;
                            machineTmp.CompletedLogging = 3;
                        }
                    }
                    #endregion
                    ReactCompleteLog(_view.tagMainBlock + "ST3_1LoggingApp", machineTmp.CompletedLogging);
                    _view.machine3 = machineTmp;
                }

                if (machine.Id == 4)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine4;
                    _view.machine4.MessageResult = string.Empty;

                    #region Check Dupplicate Item code
                    var tagName = _view.tagMainBlock + "ST3_2Code";
                    var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                    var loggings = _serviceTraceLog.GetListByItemCode(value.ToString()).Result.Where(x => x.MachineId == machine.Id);

                    if (loggings.Count() > 0)
                    {
                        machineTmp.MessageResult = string.Format("Code :{0} is dupplicated.", value);
                        machineTmp.CompletedLogging = 2;
                    }
                    else
                    {
                        try
                        {
                            keepLog = await KeepLogForMachine4(r, machine);
                            machineTmp.CompletedLogging = 1;
                        }
                        catch (Exception ex)
                        {
                            _view.machine4.MessageResult = ex.Message;
                            machineTmp.CompletedLogging = 3;
                        }
                    }
                    #endregion
                    ReactCompleteLog(_view.tagMainBlock + "ST3_2LoggingApp", machineTmp.CompletedLogging);
                    _view.machine4 = machineTmp;
                }

                //Station4
                if (machine.Id == 5)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine5;
                    _view.machine5.MessageResult = string.Empty;

                    #region Check Dupplicate Item code
                    var tagName = _view.tagMainBlock + "ST4Code";
                    var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                    var loggings = _serviceTraceLog.GetListByItemCode(value.ToString()).Result.Where(x => x.MachineId == machine.Id);

                    if (loggings.Count() > 0)
                    {
                        machineTmp.MessageResult = string.Format("Code :{0} is dupplicated.", value);
                        machineTmp.CompletedLogging = 2;
                    }
                    else
                    {
                        try
                        {
                            keepLog = await KeepLogForMachine5(r, machine, machineTags);
                            machineTmp.CompletedLogging = 1;
                        }
                        catch (Exception ex)
                        {
                            _view.machine5.MessageResult = ex.Message;
                            machineTmp.CompletedLogging = 3;
                        }
                    }
                    #endregion
                    ReactCompleteLog(_view.tagMainBlock + "ST4LoggingApp", machineTmp.CompletedLogging);
                    _view.machine5 = machineTmp;
                }

                //Station5
                if (machine.Id == 6)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine6;
                    _view.machine6.MessageResult = string.Empty;

                    #region Check Dupplicate Item code
                    var tagName = _view.tagMainBlock + "ST5_1Code";
                    var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                    var loggings = _serviceTraceLog.GetListByItemCode(value.ToString()).Result.Where(x => x.MachineId == machine.Id);

                    if (loggings.Count() > 0)
                    {
                        machineTmp.MessageResult = string.Format("Code :{0} is dupplicated.", value);
                        machineTmp.CompletedLogging = 2;
                    }
                    else
                    {
                        try
                        {
                            keepLog = await KeepLogForMachine6(r, machine, machineTags);
                            machineTmp.CompletedLogging = 1;
                        }
                        catch (Exception ex)
                        {
                            _view.machine6.MessageResult = ex.Message;
                            machineTmp.CompletedLogging = 3;
                        }
                    }
                    #endregion
                    ReactCompleteLog(_view.tagMainBlock + "ST5_1LoggingApp", machineTmp.CompletedLogging);
                    _view.machine6 = machineTmp;
                }

                if (machine.Id == 7)
                {
                    bool keepLog = false;
                    var machineTmp = _view.machine7;
                    _view.machine7.MessageResult = string.Empty;

                    #region Check Dupplicate Item code
                    var tagName = _view.tagMainBlock + "ST5_2Code";
                    var value = result.Where(x => x.ItemName == tagName).FirstOrDefault().Value;

                    var loggings = _serviceTraceLog.GetListByItemCode(value.ToString()).Result.Where(x => x.MachineId == machine.Id);

                    if (loggings.Count() > 0)
                    {
                        machineTmp.MessageResult = string.Format("Code :{0} is dupplicated.", value);
                        machineTmp.CompletedLogging = 2;
                    }
                    else
                    {
                        try
                        {
                            keepLog = await KeepLogForMachine7(r, machine, machineTags);
                            machineTmp.CompletedLogging = 1;
                        }
                        catch (Exception ex)
                        {
                            _view.machine7.MessageResult = ex.Message;
                            machineTmp.CompletedLogging = 3;
                        }
                        if (keepLog)
                        {
                            ReactCompleteLog(_view.tagMainBlock + "ST5_2LoggingApp", 1);
                        }
                    }
                    #endregion

                    _view.machine7 = machineTmp;
                }
            }               
        }

        private async Task<bool> KeepLogForMachine1(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
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

            trace.StationId = m.StationId;
            trace.MachineId = m.Id;

            foreach (var item in r)
            {
                tmpMsg = string.Empty;
                if (item.ItemName == _view.tagMainBlock + "ST1Code")
                    trace.ItemCode = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST1Final_Judgment")
                    trace.FinalResult = Convert.ToInt32(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST1ModelRunning")
                    trace.ModelRunning = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST1RepairTime")
                    trace.RepairTime = Convert.ToInt32(item.Value);

                if(InvalidDataTag(string.IsNullOrEmpty(item.Value.ToString()) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }
            }

            //Keep part Assemblies
            trace.PartAssemblies = new List<PartAssemblyModel>();
            foreach (var item in r.Where(x => tagsPart.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                tmpMsg = string.Empty;
                PartAssemblyModel part = new PartAssemblyModel();
                if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[0]")
                {
                    part.PartName = "UPR Actuator P/N";
                    part.SerialNumber = item.Value.ToString();
                }
                if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[1]")
                {
                    part.PartName = "UPR Actuator S/N";
                    part.SerialNumber = item.Value.ToString();
                }
                if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[2]")
                {
                    part.PartName = "UPR Frame";
                    part.SerialNumber = item.Value.ToString();
                }
                if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[3]")
                {
                    part.PartName = "UPR vane set LH";
                    part.SerialNumber = item.Value.ToString();
                }
                if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[4]")
                {
                    part.PartName = "UPR vane set RH";
                    part.SerialNumber = item.Value.ToString();
                }
                if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[5]")
                {
                    part.PartName = "Pull rod,";
                    part.SerialNumber = item.Value.ToString();
                }
                if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[6]")
                {
                    part.PartName = "Lever, 4";
                    part.SerialNumber = item.Value.ToString();
                }
                if (item.ItemName == _view.tagMainBlock + "ST1PartSerialNo[7]")
                {
                    part.PartName = "Space";
                    part.SerialNumber = item.Value.ToString();
                }

                if (InvalidDataTag(string.IsNullOrEmpty(item.Value.ToString()) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }

                trace.PartAssemblies.Add(part);
            }

            //Keep Tightening
            int i = 1;
            trace.TighteningResults = new List<TighteningResultModel>();
            foreach (var item in r.Where(x => tagsTightening.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                TighteningResultModel t = new TighteningResultModel();
                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[0]")
                {
                    t.No = i.ToString();
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[0]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[0]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[0]").FirstOrDefault().Value);
                    t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[0]").FirstOrDefault().Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[1]")
                {
                    t.No = i.ToString();
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[1]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[1]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[1]").FirstOrDefault().Value);
                    t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[1]").FirstOrDefault().Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[2]")
                {
                    t.No = i.ToString();
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[2]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[2]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[2]").FirstOrDefault().Value);
                    t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[2]").FirstOrDefault().Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[3]")
                {
                    t.No = i.ToString();
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[3]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[3]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[3]").FirstOrDefault().Value);
                    t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[3]").FirstOrDefault().Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[4]")
                {
                    t.No = i.ToString();
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[4]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[4]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[4]").FirstOrDefault().Value);
                    t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[4]").FirstOrDefault().Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[5]")
                {
                    t.No = i.ToString();
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[5]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[5]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[5]").FirstOrDefault().Value);
                    t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[5]").FirstOrDefault().Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[6]")
                {
                    t.No = i.ToString();
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[6]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[6]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[6]").FirstOrDefault().Value);
                    t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[6]").FirstOrDefault().Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[7]")
                {
                    t.No = i.ToString();
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[7]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[7]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[7]").FirstOrDefault().Value);
                    t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[7]").FirstOrDefault().Value.ToString();
                }

                if (InvalidDataTag(string.IsNullOrEmpty(item.Value.ToString()) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }

                trace.TighteningResults.Add(t);
                i++;
            }

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

                if (InvalidDataTag(string.IsNullOrEmpty(item.Value.ToString()) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }

                trace.CameraResults.Add(cam);
            }

            if (invalid)
            {
                var mac = _view.machine1;
                mac.MessageResult = errMsg;
                mac.CompletedLogging = 2;
                _view.machine1 = mac;
                result = false;
            }
            else
            {
                try
                {
                    await _serviceTraceLog.Create(trace);
                }
                catch
                {
                    result = false;
                }
            }

            return result;            
        }


        private async Task<bool> KeepLogForMachine2(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
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

            foreach (var item in r)
            {
                if (item.ItemName == _view.tagMainBlock + "ST2Code")
                    trace.ItemCode = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST2Final_Judgment")
                    trace.FinalResult = Convert.ToInt32(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST2RepairTime")
                    trace.RepairTime = Convert.ToInt32(item.Value);

                if (InvalidDataTag(string.IsNullOrEmpty(item.Value.ToString()) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }
            }

            //Keep part Assemblies
            trace.PartAssemblies = new List<PartAssemblyModel>();
            foreach (var item in r.Where(x => tagsPart.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                PartAssemblyModel part = new PartAssemblyModel();
                if (item.ItemName == _view.tagMainBlock + "ST2PartSerialNo[0]")
                {
                    part.PartName = "Z support LH";
                    part.SerialNumber = item.Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST2PartSerialNo[1]")
                {
                    part.PartName = "Z support RH";
                    part.SerialNumber = item.Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST2PartSerialNo[2]")
                {
                    part.PartName = "Blind vane";
                    part.SerialNumber = item.Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST2PartSerialNo[3]")
                {
                    part.PartName = "Blind vane";
                    part.SerialNumber = item.Value.ToString();
                }

                if (InvalidDataTag(string.IsNullOrEmpty(item.Value.ToString()) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }

                trace.PartAssemblies.Add(part);
            }

            //Keep Tightening
            int i = 1;
            trace.TighteningResults = new List<TighteningResultModel>();
            foreach (var item in r.Where(x => tagsTightening.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                TighteningResultModel t = new TighteningResultModel();
                if (item.ItemName == _view.tagMainBlock + "ST2TestResult[0]")
                {
                    t.No = "Hollow Nut Screw L";
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[0]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[0]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[0]").FirstOrDefault().Value);
                    t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[0]").FirstOrDefault().Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST2TestResult[1]")
                {
                    t.No = "Hollow Nut Screw R";
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[1]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[1]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[1]").FirstOrDefault().Value);
                    t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[1]").FirstOrDefault().Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST2TestResult[2]")
                {
                    t.No = "Ejot Screw L";
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[2]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[2]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[2]").FirstOrDefault().Value);
                    t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[2]").FirstOrDefault().Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST2TestResult[3]")
                {
                    t.No = "Ejot Screw R";
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[3]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[3]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[3]").FirstOrDefault().Value);
                    t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[3]").FirstOrDefault().Value.ToString();
                }

                if (InvalidDataTag(string.IsNullOrEmpty(item.Value.ToString()) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }

                trace.TighteningResults.Add(t);
                i++;
            }

            if (invalid)
            {
                var mac = _view.machine1;
                mac.MessageResult = errMsg;
                mac.CompletedLogging = 2;
                _view.machine1 = mac;
                result = false;
            }
            else
            {
                try
                {
                    await _serviceTraceLog.Create(trace);
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        private async Task<bool> KeepLogForMachine7(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
        {
            bool result = true;
            bool invalid = false;
            string errMsg = string.Empty;
            string tmpMsg = string.Empty;

            TraceabilityLogModel trace = new TraceabilityLogModel();
            trace.StationId = m.StationId;
            trace.MachineId = m.Id;
            trace.Description = "Lower EOL laser marking and probe sensor check";

            foreach (var item in r)
            {
                if (item.ItemName == _view.tagMainBlock + "ST5_2Code")
                    trace.ItemCode = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST5_2ProductSerialNo")
                    trace.PartSerialNumber = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[0]")
                    trace.Actuator = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[1]")
                    trace.ProductionDate = Convert.ToDateTime(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[2]")
                    trace.SwNumber = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[3]")
                    trace.LineErrorCounter = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[4]")
                    trace.CurrentMaximum = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST5_2TestResult[5]")
                    trace.OpenAngle = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST5_2Final_Judgment")
                    trace.FinalResult = Convert.ToInt32(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST5_2RepairTime")
                    trace.RepairTime = Convert.ToInt32(item.Value);

                if (InvalidDataTag(string.IsNullOrEmpty(item.Value.ToString()) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }
            }

            if (invalid)
            {
                var mac = _view.machine7;
                mac.MessageResult = errMsg;
                mac.CompletedLogging = 2;
                _view.machine7 = mac;
                result = false;
            }
            else
            {
                try
                {
                    await _serviceTraceLog.Create(trace);
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        private async Task<bool> KeepLogForMachine6(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
        {
            bool result = true;
            bool invalid = false;
            string errMsg = string.Empty;
            string tmpMsg = string.Empty;

            TraceabilityLogModel trace = new TraceabilityLogModel();
            trace.StationId = m.StationId;
            trace.MachineId = m.Id;
            trace.Description = "Upper  EOL laser marking and probe sensor check";

            foreach (var item in r)
            {
                if (item.ItemName == _view.tagMainBlock + "ST5_1Code")
                    trace.ItemCode = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST5_1ProductSerialNo")
                    trace.PartSerialNumber = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[0]")
                    trace.Actuator = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[1]")
                    trace.ProductionDate = Convert.ToDateTime(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[2]")
                    trace.SwNumber = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[3]")
                    trace.LineErrorCounter = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[4]")
                    trace.CurrentMaximum = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST5_1TestResult[5]")
                    trace.OpenAngle = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST5_1Final_Judgment")
                    trace.FinalResult = Convert.ToInt32(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST5_1RepairTime")
                    trace.RepairTime = Convert.ToInt32(item.Value);

                if (InvalidDataTag(string.IsNullOrEmpty(item.Value.ToString()) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }
            }

            if (invalid)
            {
                var mac = _view.machine6;
                mac.MessageResult = errMsg;
                mac.CompletedLogging = 2;
                _view.machine6 = mac;
                result = false;
            }
            else
            {
                try
                {
                    await _serviceTraceLog.Create(trace);
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Station 4 Semi -auto Tightening lower Frame Support M6
        /// </summary>
        /// <param name="r"></param>
        /// <param name="m"></param>
        private async Task<bool> KeepLogForMachine5(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
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

            foreach (var item in r)
            {
                if (item.ItemName == _view.tagMainBlock + "ST4Code")
                    trace.ItemCode = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST4Final_Judgment")
                    trace.FinalResult = Convert.ToInt32(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST4ModelRunning")
                    trace.ModelRunning = item.Value.ToString();

                if (InvalidDataTag(string.IsNullOrEmpty(item.Value.ToString()) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }
            }

            //Keep part Assemblies
            trace.PartAssemblies = new List<PartAssemblyModel>();
            foreach (var item in r.Where(x => tagsPart.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                PartAssemblyModel part = new PartAssemblyModel();
                if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[0]")
                {
                    part.PartName = "LWR Actuator P/N";
                    part.SerialNumber = item.Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[1]")
                {
                    part.PartName = "LWR Actuator S/N";
                    part.SerialNumber = item.Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[2]")
                {
                    part.PartName = "LWR Frame";
                    part.SerialNumber = item.Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[3]")
                {
                    part.PartName = "Vane LH";
                    part.SerialNumber = item.Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST4PartSerialNo[4]")
                {
                    part.PartName = "Vane RH";
                    part.SerialNumber = item.Value.ToString();
                }

                if (InvalidDataTag(string.IsNullOrEmpty(item.Value.ToString()) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }

                trace.PartAssemblies.Add(part);
            }

            //Keep Tightening
            int i = 1;
            trace.TighteningResults = new List<TighteningResultModel>();
            foreach (var item in r.Where(x => tagsTightening.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                TighteningResultModel t = new TighteningResultModel();
                if (item.ItemName == _view.tagMainBlock + "ST4TestResult[0]")
                {                    
                    t.No = i.ToString();
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter2[0]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter1[0]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter3[0]").FirstOrDefault().Value);
                    t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST4TestJudgment[0]").FirstOrDefault().Value.ToString();
                }

                if (item.ItemName == _view.tagMainBlock + "ST4TestResult[1]")
                {
                    t.No = i.ToString();
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter2[1]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter1[1]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter3[1]").FirstOrDefault().Value);
                    t.TestResult = r.Where(x => x.ItemName == _view.tagMainBlock + "ST4TestJudgment[1]").FirstOrDefault().Value.ToString();
                }

                if (InvalidDataTag(string.IsNullOrEmpty(item.Value.ToString()) ? "" : item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }

                trace.TighteningResults.Add(t);
                i++;
            }

            if (invalid)
            {
                var mac = _view.machine5;
                mac.MessageResult = errMsg;
                mac.CompletedLogging = 2;
                _view.machine5 = mac;
                result = false;
            }
            else
            {
                try
                {
                    await _serviceTraceLog.Create(trace);
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// STATION 3 LOWER AGS FRAM Auto gauge check
        /// </summary>
        /// <param name="r"></param>
        /// <param name="m"></param>
        private async Task<bool> KeepLogForMachine4(IEnumerable<ItemValueResult> r, MachineModel m)
        {
            bool result = true;
            bool invalid = false;
            string errMsg = string.Empty;
            string tmpMsg = string.Empty;

            TraceabilityLogModel trace = new TraceabilityLogModel();
            trace.StationId = m.StationId;
            trace.MachineId = m.Id;
            trace.Description = "LOWER AGS FRAM";

            foreach (var item in r)
            {
                if (item.ItemName == _view.tagMainBlock + "ST3_2Code")
                    trace.ItemCode = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST3_2TestResult[0]")
                    trace.Attribute1 = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST3_2Final_Judgment")
                    trace.FinalResult = Convert.ToInt32(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST3_2RepairTime")
                    trace.RepairTime = Convert.ToInt32(item.Value);

                if (InvalidDataTag(item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }
            }

            if (invalid)
            {
                var mac = _view.machine4;
                mac.MessageResult = errMsg;
                mac.CompletedLogging = 2;
                _view.machine4 = mac;
                result = false;
            }
            else
            {
                try
                {
                    await _serviceTraceLog.Create(trace);
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// STATION 3 UPPER AGS FRAM Auto gauge check
        /// </summary>
        /// <param name="r"></param>
        /// <param name="m"></param>
        private async Task<bool> KeepLogForMachine3(IEnumerable<ItemValueResult> r, MachineModel m)
        {
            bool result = true;
            bool invalid = false;
            string errMsg = string.Empty;
            string tmpMsg = string.Empty;

            TraceabilityLogModel trace = new TraceabilityLogModel();
            trace.StationId = m.StationId;
            trace.MachineId = m.Id;
            trace.Description = "UPPER AGS FRAM";

            foreach (var item in r)
            {
                if (item.ItemName == _view.tagMainBlock + "ST3_1Code")
                    trace.ItemCode = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST3_1TestResult[0]")
                    trace.Attribute1 = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST3_1Final_Judgment")
                    trace.FinalResult = Convert.ToInt32(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST3_1RepairTime")
                    trace.RepairTime = Convert.ToInt32(item.Value);

                if (InvalidDataTag(item.Value.ToString(), item.ItemName, out tmpMsg))
                {
                    invalid = true;
                    if (!string.IsNullOrEmpty(errMsg))
                        errMsg += Environment.NewLine;

                    errMsg += tmpMsg;
                }
            }

            if (invalid)
            {
                var mac = _view.machine3;
                mac.MessageResult = errMsg;
                mac.CompletedLogging = 2;
                _view.machine3 = mac;
                result = false;
            }
            else
            {
                try
                {
                    await _serviceTraceLog.Create(trace);
                }
                catch
                {
                    result = false;
                }
            }

            return result;
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

            Task t = Task.Run(() => {
                result = WriteWord(_view.tagClockReady, 1);
            });
            TimeSpan ts = TimeSpan.FromMilliseconds(2000);
            //if (!t.Wait(ts))
            //    Console.WriteLine("The timeout interval elapsed.");

            if (!t.Wait(ts))
            {
                _view.connectedPlc = false;
                _view.systemReady = false;
                MessageBox.Show("Connection failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Application.Exit();
            }
        }

        private bool ReactCompleteLog(string tagName, int val = 1)
        {
            bool result = false;

            Task t = Task.Run(() => {
                result = WriteWord(tagName, val);
            });
            TimeSpan ts = TimeSpan.FromMilliseconds(2000);

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
            }
        }

        private bool Disconnect()
        {
            if (_view.daServer != null && _view.daServer.IsConnected)
                _view.daServer.Disconnect();

            return true;
        }

        private async void FormLoad(object sender, EventArgs e)
        {

            _view.serverUrl = ConfigurationManager.AppSettings["DefaultUrl"].ToString();
            _view.tagMainBlock = ConfigurationManager.AppSettings["MainBlock"].ToString();

            var machines = await _serviceMachine.GetAll();
            int i = 1;
            foreach(var mac in machines.ToList().OrderBy(o => o.Id))
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

            var result = await _servicePLCTag.GetAll();
            _view.plcTags = result.ToList();

            if(_view.plcTags.Count > 0)
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
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Connect OPC Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                foreach(var tag in _view.plcTags)
                {
                    _view.items[i] = new Item();
                    _view.items[i].ItemName = _view.tagMainBlock + tag.PlcTag;

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
            }catch
            {
                return false;
            }
            
        }

        [Obsolete]
        private bool KeeppingFile(string itemCode, out string outTargetFilePath, out string outErrorMessage)
        {
            string sourceImagePath = ConfigurationSettings.AppSettings["sourcePath"].ToString();
            string targetImagePath = ConfigurationSettings.AppSettings["targetPath"].ToString();

            bool result = false;
            string _errMsg = string.Empty;
            string fileName = string.Empty;
            string targetFile = string.Format(@"{0}\\{1}", targetImagePath, DateTime.Now.ToString("dd-MM-yyyy"));
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
                        
            if(tag != null)
            {
                if(tag.DataType.ToUpper() == "STRING")
                {
                    int len1 = value.Length;
                    int len2 = tag.Length;
                    if (InvalidString(len1, len2))
                    {
                        returnMsg = tag.Description + " tag must be " + len2 + " digits.";
                        result = true;
                    }
                }

                if (tag.DataType.ToUpper() == "DECIMAL")
                {
                    if (InvalidDecimal(value))
                    {
                        returnMsg = tag.Description + " tag must be Decimal type.";
                        result = true;
                    }
                }

                if (tag.DataType.ToUpper() == "INT")
                {
                    if (InvalidInt(value))
                    {
                        returnMsg = tag.Description + " tag must be Integer type.";
                        result = true;
                    }
                }

                if (tag.DataType.ToUpper() == "BOOL")
                {
                    if (InvalidBoolean(value))
                    {
                        returnMsg = tag.Description + " tag must be Boolean type.";
                        result = true;
                    }
                }
            }


            tmpMsg = returnMsg;
            return result;
        }

        private bool InvalidString(int len1, int len2)
        {
            return len1 == len2;
        }

        private bool InvalidDecimal(string number)
        {
            decimal myDecimal;
            return decimal.TryParse(number, out myDecimal);
        }

        private bool InvalidInt(string number)
        {
            int myInt;
            return int.TryParse(number, out myInt);
        }

        private bool InvalidBoolean(string number)
        {
            return !(number == "0" || number == "1");
        }
    }
}
