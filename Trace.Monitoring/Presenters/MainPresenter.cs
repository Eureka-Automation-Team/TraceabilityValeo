using Opc.Da;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
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
        }

        private void KeepLogging(object sender, EventArgs e)
        {
            if (_view.systemReady)
            {
                MachineModel machine = (MachineModel)sender;
                var result = _view.groupRead.Read(_view.groupRead.Items).ToList();

                var machineTags = _servicePLCTag.GetAll().Result.ToList().Where(x => x.MachineId == machine.Id);
                var tags = (from tag in machineTags
                            where tag.MachineId == machine.Id
                            select new { Tag = _view.tagMainBlock + tag.PlcTag, Type = tag.TypeCode }).ToArray();

                var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));

                //Station1
                if (machine.Id == 1)
                {
                    KeepLogForMachine1(r, machine, machineTags);
                    WriteWord(_view.tagMainBlock +  "ST1LoggingApp", 1);
                }

                //Station2
                if (machine.Id == 2)
                {
                    KeepLogForMachine2(r, machine, machineTags);
                    WriteWord(_view.tagMainBlock + "ST2LoggingApp", 1);
                }

                //Station3
                if (machine.Id == 3)
                {
                    KeepLogForMachine3(r, machine);
                    WriteWord(_view.tagMainBlock + "ST3_1LoggingApp", 1);
                }

                if (machine.Id == 4)
                {
                    KeepLogForMachine4(r, machine);
                    WriteWord(_view.tagMainBlock + "ST3_2LoggingApp", 1);
                }

                //Station4
                if (machine.Id == 5)
                {
                    KeepLogForMachine5(r, machine, machineTags);
                    WriteWord(_view.tagMainBlock + "ST4LoggingApp", 1);
                }

                //Station5
                if (machine.Id == 6)
                {
                    KeepLogForMachine6(r, machine, machineTags);
                    WriteWord(_view.tagMainBlock + "ST5_1LoggingApp", 1);
                }

                if (machine.Id == 7)
                {
                    KeepLogForMachine7(r, machine, machineTags);
                    WriteWord(_view.tagMainBlock + "ST5_2LoggingApp", 1);
                }
            }               
        }

        private void KeepLogForMachine1(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
        {
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
                if (item.ItemName == _view.tagMainBlock + "ST1Code")
                    trace.ItemCode = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST1Final_Judgment")
                    trace.FinalResult = Convert.ToBoolean(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST1ModelRunning")
                    trace.ModelRunning = item.Value.ToString();

                if (item.ItemName == _view.tagMainBlock + "ST1RepairTime")
                    trace.RepairTime = Convert.ToInt32(item.Value);
            }

            //Keep part Assemblies
            foreach (var item in r.Where(x => tagsPart.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
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

                trace.PartAssemblies.Add(part);
            }

            //Keep Tightening
            int i = 1;
            foreach (var item in r.Where(x => tagsTightening.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                TighteningResultModel t = new TighteningResultModel();
                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[0]")
                {
                    t.No = i;
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[0]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[0]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[0]").FirstOrDefault().Value);
                    t.TestResult = Convert.ToBoolean(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[0]").FirstOrDefault().Value);
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[1]")
                {
                    t.No = i;
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[1]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[1]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[1]").FirstOrDefault().Value);
                    t.TestResult = Convert.ToBoolean(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[1]").FirstOrDefault().Value);
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[2]")
                {
                    t.No = i;
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[2]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[2]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[2]").FirstOrDefault().Value);
                    t.TestResult = Convert.ToBoolean(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[2]").FirstOrDefault().Value);
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[3]")
                {
                    t.No = i;
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[3]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[3]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[3]").FirstOrDefault().Value);
                    t.TestResult = Convert.ToBoolean(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[3]").FirstOrDefault().Value);
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[4]")
                {
                    t.No = i;
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[4]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[4]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[4]").FirstOrDefault().Value);
                    t.TestResult = Convert.ToBoolean(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[4]").FirstOrDefault().Value);
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[5]")
                {
                    t.No = i;
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[5]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[5]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[5]").FirstOrDefault().Value);
                    t.TestResult = Convert.ToBoolean(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[5]").FirstOrDefault().Value);
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[6]")
                {
                    t.No = i;
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[6]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[6]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[6]").FirstOrDefault().Value);
                    t.TestResult = Convert.ToBoolean(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[6]").FirstOrDefault().Value);
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[7]")
                {
                    t.No = i;
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter2[7]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter1[7]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1Parameter3[7]").FirstOrDefault().Value);
                    t.TestResult = Convert.ToBoolean(r.Where(x => x.ItemName == _view.tagMainBlock + "ST1TestJudgment[7]").FirstOrDefault().Value);
                }

                trace.TighteningResults.Add(t);
                i++;
            }

            foreach (var item in r.Where(x => tagsCamera.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                CameraResultModel cam = new CameraResultModel();
                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[8]")
                {
                    cam.CameraName = "Lever Assy L";
                    cam.TestResult = Convert.ToBoolean(item.Value);
                }

                if (item.ItemName == _view.tagMainBlock + "ST1TestResult[9]")
                {
                    cam.CameraName = "Lever Assy R";
                    cam.TestResult = Convert.ToBoolean(item.Value);
                }

                trace.CameraResults.Add(cam);
            }

            _serviceTraceLog.Create(trace);
        }

        private void KeepLogForMachine2(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
        {
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
                    trace.FinalResult = Convert.ToBoolean(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST2RepairTime")
                    trace.RepairTime = Convert.ToInt32(item.Value);
            }

            //Keep part Assemblies
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

                trace.PartAssemblies.Add(part);
            }

            //Keep Tightening
            int i = 1;
            foreach (var item in r.Where(x => tagsTightening.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                TighteningResultModel t = new TighteningResultModel();
                if (item.ItemName == _view.tagMainBlock + "ST2TestResult[0]")
                {
                    t.No = i;
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[0]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[0]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[0]").FirstOrDefault().Value);
                    t.TestResult = Convert.ToBoolean(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[0]").FirstOrDefault().Value);
                }

                if (item.ItemName == _view.tagMainBlock + "ST2TestResult[1]")
                {
                    t.No = i;
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[1]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[1]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[1]").FirstOrDefault().Value);
                    t.TestResult = Convert.ToBoolean(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[1]").FirstOrDefault().Value);
                }

                if (item.ItemName == _view.tagMainBlock + "ST2TestResult[2]")
                {
                    t.No = i;
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[2]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[2]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[2]").FirstOrDefault().Value);
                    t.TestResult = Convert.ToBoolean(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[2]").FirstOrDefault().Value);
                }

                if (item.ItemName == _view.tagMainBlock + "ST2TestResult[3]")
                {
                    t.No = i;
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter2[3]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter1[3]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2Parameter3[3]").FirstOrDefault().Value);
                    t.TestResult = Convert.ToBoolean(r.Where(x => x.ItemName == _view.tagMainBlock + "ST2TestJudgment[3]").FirstOrDefault().Value);
                }

                trace.TighteningResults.Add(t);
                i++;
            }

            _serviceTraceLog.Create(trace);
        }

        private void KeepLogForMachine7(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
        {
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
                    trace.FinalResult = Convert.ToBoolean(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST5_2RepairTime")
                    trace.RepairTime = Convert.ToInt32(item.Value);
            }

            _serviceTraceLog.Create(trace);
        }

        private void KeepLogForMachine6(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
        {
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
                    trace.FinalResult = Convert.ToBoolean(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST5_1RepairTime")
                    trace.RepairTime = Convert.ToInt32(item.Value);
            }

            _serviceTraceLog.Create(trace);
        }

        /// <summary>
        /// Station 4 Semi -auto Tightening lower Frame Support M6
        /// </summary>
        /// <param name="r"></param>
        /// <param name="m"></param>
        private void KeepLogForMachine5(IEnumerable<ItemValueResult> r, MachineModel m, IEnumerable<PlcTagModel> machineTags)
        {
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
                    trace.FinalResult = Convert.ToBoolean(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST4ModelRunning")
                    trace.ModelRunning = item.Value.ToString();
            }

            //Keep part Assemblies
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
                trace.PartAssemblies.Add(part);
            }

            //Keep Tightening
            int i = 1;
            foreach (var item in r.Where(x => tagsTightening.Any(s => s.Tag == x.ItemName)).OrderBy(o => o.ItemName))
            {
                TighteningResultModel t = new TighteningResultModel();
                if (item.ItemName == _view.tagMainBlock + "ST4TestResult[0]")
                {                    
                    t.No = i;
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter2[0]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter1[0]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter3[0]").FirstOrDefault().Value);
                    t.TestResult = Convert.ToBoolean(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4TestJudgment[0]").FirstOrDefault().Value);
                }

                if (item.ItemName == _view.tagMainBlock + "ST4TestResult[1]")
                {
                    t.No = i;
                    t.Result = Convert.ToDecimal(item.Value);
                    t.Min = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter2[1]").FirstOrDefault().Value);
                    t.Max = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter1[1]").FirstOrDefault().Value);
                    t.Target = Convert.ToDecimal(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4Parameter3[1]").FirstOrDefault().Value);
                    t.TestResult = Convert.ToBoolean(r.Where(x => x.ItemName == _view.tagMainBlock + "ST4TestJudgment[1]").FirstOrDefault().Value);
                }

                trace.TighteningResults.Add(t);
                i++;
            }

                _serviceTraceLog.Create(trace);
        }

        /// <summary>
        /// STATION 3 LOWER AGS FRAM Auto gauge check
        /// </summary>
        /// <param name="r"></param>
        /// <param name="m"></param>
        private void KeepLogForMachine4(IEnumerable<ItemValueResult> r, MachineModel m)
        {
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
                    trace.FinalResult = Convert.ToBoolean(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST3_2RepairTime")
                    trace.RepairTime = Convert.ToInt32(item.Value);
            }

            _serviceTraceLog.Create(trace);
        }

        /// <summary>
        /// STATION 3 UPPER AGS FRAM Auto gauge check
        /// </summary>
        /// <param name="r"></param>
        /// <param name="m"></param>
        private void KeepLogForMachine3(IEnumerable<ItemValueResult> r, MachineModel m)
        {
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
                    trace.FinalResult = Convert.ToBoolean(item.Value);

                if (item.ItemName == _view.tagMainBlock + "ST3_1RepairTime")
                    trace.RepairTime = Convert.ToInt32(item.Value);
            }

            _serviceTraceLog.Create(trace);
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
            }
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

        [Obsolete]
        private async void FormLoad(object sender, EventArgs e)
        {

            _view.serverUrl = ConfigurationSettings.AppSettings["DefaultUrl"].ToString();
            _view.tagMainBlock = ConfigurationSettings.AppSettings["MainBlock"].ToString();

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
                _view.daServer.Connect(url, new Opc.ConnectData(new System.Net.NetworkCredential()));
                //Console.WriteLine(String.Format("Connect to server {0}", serverName));

                //3rd Create a group if items            
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
            }catch(Exception ex)
            {
                return false;
            }
            
        }

    }
}
