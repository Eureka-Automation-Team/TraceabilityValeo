using MySql.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trace.Domain.Models;

namespace Trace.Data
{
    // Code-Based Configuration and Dependency resolution
    [DbConfigurationType(typeof(MySqlEFConfiguration))]

    public class TraceDbContext : DbContext
    {
        public TraceDbContext(string connString) :
            base(connString)
        {
        }

        public DbSet<MachineModel> Machines { get; set; }
        public DbSet<StationModel> Stations { get; set; }
        public DbSet<PartAssemblyModel> PartAssemblies { get; set; }
        public DbSet<TighteningResultModel> TighteningResults { get; set; }
        public DbSet<CameraResultModel> CameraResults { get; set; }
        public DbSet<TraceabilityLogModel> TraceabilityLogs { get; set; }
        public DbSet<PlcTagModel> PlcTags { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ///machines
            modelBuilder.Entity<MachineModel>().ToTable("machines")
                .HasKey(e => e.Id);
            modelBuilder.Entity<MachineModel>()
                .Property(e => e.ManchineName).IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<MachineModel>()
                .Property(e => e.ModelName).HasMaxLength(100);

            ///stations
            modelBuilder.Entity<StationModel>().ToTable("stations")
                .HasKey(e => e.Id);
            modelBuilder.Entity<StationModel>()
                .Property(e => e.StationNumber).IsRequired()
                .HasMaxLength(25);

            ///part_assemblies
            modelBuilder.Entity<PartAssemblyModel>().ToTable("part_assemblies")
                .HasKey(e => e.Id);
            modelBuilder.Entity<PartAssemblyModel>()
                .Property(e => e.PartName).IsRequired()
                .HasMaxLength(250);
            modelBuilder.Entity<PartAssemblyModel>()
                .Property(e => e.SerialNumber).IsRequired()
                .HasMaxLength(500);

            ///tightening_results
            modelBuilder.Entity<TighteningResultModel>().ToTable("tightening_results")
                .HasKey(e => e.Id);
            modelBuilder.Entity<TighteningResultModel>()
                .Property(e => e.No).IsRequired();
            modelBuilder.Entity<TighteningResultModel>()
                .Property(e => e.Min).HasPrecision(15, 2);
            modelBuilder.Entity<TighteningResultModel>()
                .Property(e => e.Max).HasPrecision(15, 2);
            modelBuilder.Entity<TighteningResultModel>()
                .Property(e => e.Target).HasPrecision(15, 2);
            modelBuilder.Entity<TighteningResultModel>()
                .Property(e => e.Result).HasPrecision(15, 2);

            ///camera_results
            modelBuilder.Entity<CameraResultModel>().ToTable("camera_results")
                .HasKey(e => e.Id);
            modelBuilder.Entity<CameraResultModel>()
                .Property(e => e.CameraName).IsRequired()
                .HasMaxLength(250);

            ///traceability_logs
            modelBuilder.Entity<TraceabilityLogModel>().ToTable("traceability_logs")
                .HasKey(e => e.Id);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.ItemCode).IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.ModelRunning).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.Description).HasMaxLength(500);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.PartSerialNumber).HasMaxLength(200);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.Actuator).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.SwNumber).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.CurrentMaximum).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.OpenAngle).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.LineErrorCounter).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.Attribute1).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.Attribute2).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.Attribute3).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.Attribute4).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.Attribute5).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.Attribute6).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.Attribute7).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.Attribute8).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.Attribute9).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.Attribute10).HasMaxLength(100);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.ImagePath).HasMaxLength(500);
            modelBuilder.Entity<TraceabilityLogModel>()
                .Property(e => e.QRCodePath).HasMaxLength(500);

            ///plc_tags
            modelBuilder.Entity<PlcTagModel>().ToTable("plc_tags")
                .HasKey(e => e.Id);
            modelBuilder.Entity<PlcTagModel>()
                .Property(e => e.PlcTag).IsRequired()
                .HasMaxLength(250);
            modelBuilder.Entity<PlcTagModel>()
                .Property(e => e.Description)
                .HasMaxLength(250);
            modelBuilder.Entity<PlcTagModel>()
                .Property(e => e.DataType).IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<PlcTagModel>()
                .Property(e => e.TypeCode).IsRequired()
                .HasMaxLength(100);
        }
    }
}
