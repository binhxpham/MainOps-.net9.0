using Microsoft.EntityFrameworkCore;
using MainOps.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MainOps.Models.ReportClasses;
using MainOps.Models.CGJClasses;
using MainOps.Models.WTPClasses.MainClasses;
using MainOps.Models.WTPClasses.HelperClasses;
using MainOps.Models.WTPClasses.MixedClasses;
using MainOps.Models.CGJClassesBeton;

namespace MainOps.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().ToTable("AspNetUsers");
            builder.Entity<IdentityRole>().ToTable("AspNetRoles");

            builder.Entity<WaterSampleTypeWaterSamplePackage>()
                .HasKey(wp => new { wp.WaterSamplePackageId, wp.WaterSampleTypeId });
           builder.Entity<WaterSampleTypeWaterSamplePackage>()
                .HasOne(wp => wp.WaterSampleType)
                .WithMany(w => w.WaterSampleTypeWaterSamplePackages)
                .HasForeignKey(wp => wp.WaterSampleTypeId);
            builder.Entity<WaterSampleTypeWaterSamplePackage>()
                .HasOne(wp => wp.WaterSamplePackage)
                .WithMany(p => p.WaterSampleTypeWaterSamplePackages)
                .HasForeignKey(wp => wp.WaterSamplePackageId);
            builder.Entity<ProjectStatusProjectCategory>()
                .HasKey(wp => new { wp.ProjectStatusId, wp.ProjectCategoryId });
            builder.Entity<ProjectStatusProjectCategory>()
                 .HasOne(wp => wp.ProjectStatus)
                 .WithMany(w => w.ProjectStatusProjectCategories)
                 .HasForeignKey(wp => wp.ProjectStatusId);
            builder.Entity<ProjectStatusProjectCategory>()
                .HasOne(wp => wp.ProjectCategory)
                .WithMany(p => p.ProjectStatusProjectCategories)
                .HasForeignKey(wp => wp.ProjectCategoryId);
            builder.Entity<DecommissionableItem>()
                    .HasOne(m => m.InstallItemType)
                    .WithMany(p => p.InstalledDecommissionableItems)
                    .HasForeignKey(m => m.InstalledItemTypeId);
            builder.Entity<DecommissionableItem>()
                    .HasOne(m => m.BoQItemType)
                    .WithMany(p => p.BoQDecommissionableItems)
                    .HasForeignKey(m => m.BoQItemTypeId);
        }
        
        public DbSet<Meas> Measures { get; set; }
        public DbSet<MeasPoint> MeasPoints { get; set; }
        public DbSet<MeasType> MeasTypes { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Offset> Offsets { get; set; }
        public DbSet<Logger> Loggers { get; set; }
        public DbSet<LoggerChange> LoggerChanges { get; set; }
        public DbSet<CoordSystem> CoordSystems { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<DataPoint> DataPoints { get; set; }
        public DbSet<MonitorType> MonitorType { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<HJItem> HJItems { get; set; }
        public DbSet<HJItemClass> HJItemClasses { get; set; }
        public DbSet<HJItemMasterClass> HJItemMasterClasses { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<PumpActivity> PumpActivities { get; set; }
        public DbSet<Generator_Test> Generator_Test { get; set; }
        public DbSet<KSType> KSTypes { get; set; }
        public DbSet<WTP_Test> WTP_Tests { get; set; }
        public DbSet<Well_Installation> Well_Installations { get; set; }
        public DbSet<Well_Development> Well_Developments { get; set; }
        public DbSet<Machinery> Machinery { get; set; }
        public DbSet<Daily_Report> Daily_Reports { get; set; }
        public DbSet<Pump_Installation> Pump_Installations { get; set; }
        public DbSet<Pump_Commission> Pump_Commissions { get; set; }
        public DbSet<Well_Drilling> Well_Drillings { get; set; }
        public DbSet<TrackItem> TrackItems { get; set; }
        public DbSet<CoordTrack> CoordTracks { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<PhotoFilePack> PhotoFilesPacked { get; set; }
        public DbSet<PhotoFilePack> PhotoFilesSent { get; set; }
        public DbSet<PhotoFilePack> PhotoFilesReceived { get; set; }
        public DbSet<PhotoFilePack> PhotoFilesInstalled { get; set; }
        public DbSet<PhotoError> PhotoError { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<Install> Installations { get; set; }
        public DbSet<Arrival> Arrivals { get; set; }
        public DbSet<Mobilize> Mobilisations { get; set; }
        public DbSet<DeInstall> Deinstallations { get; set; }
        public DbSet<PhotoFileInstalled2> PhotoFilesInstalls { get; set; }
        public DbSet<PhotoFileMobilized> PhotoFilesMobilizations { get; set; }
        public DbSet<PhotoFileDeinstalled> PhotoFilesDeInstalls { get; set; }
        public DbSet<PhotoFileArrival> PhotoFilesArrivals { get; set; }
        public DbSet<CoordTrack2> CoordTrack2s { get; set; }
        public DbSet<Log2> Log2s { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Daily_Report_2> Daily_Report_2s { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<WaterSampleType> WaterSampleTypes { get; set; }
        public DbSet<WaterSampleMeas> WaterSampleMeasures { get; set; }
        public DbSet<WaterSamplePlace> WaterSamplePlaces { get; set; }
        public DbSet<WaterSampleLimit> WaterSampleLimits { get; set; }
        public DbSet<CoordTrack3> CoordTrack3s { get; set; }
        public DbSet<ThreeStepTest> ThreeStepTests { get; set; }
        public DbSet<PumpTestData> PumpTestDatas { get; set; }
        public DbSet<PumpTestPhoto> PumpTestPhotos { get; set; }
        public DbSet<ToolBox> ToolBoxes { get; set; }
        public DbSet<ToolBoxUser> ToolBoxUsers { get; set; }
        public DbSet<SmallPart> SmallParts { get; set; }
        public DbSet<SafetyProblem> SafetyProblems { get; set; }
        public DbSet<SummaryReport> SummaryReports { get; set; }
        public DbSet<AccidentReport> AccidentReports { get; set; }
        public DbSet<ExtraWork> ExtraWorks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<BoQHeadLine> BoQHeadLines { get; set; }
        public DbSet<InstallOperation> InstallOperations { get; set; }
        public DbSet<Daily_Report_2_temp> Daily_Reports_Ongoing { get; set; }
        public DbSet<SubProject> SubProjects { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<PhotoFileMaintenance> PhotoFilesMaintenance { get; set; }
        public DbSet<MaintenanceType> MaintenanceTypes { get; set; }
        public DbSet<MaintenanceSubType> MaintenanceSubTypes { get; set; }
        public DbSet<PumpTestDataDevice> PumpTestDatasDevice { get; set; }
        public DbSet<ClearPumpTestDataDevice> ClearPumpTestDatasDevice { get; set; }
        public DbSet<WTPCheck> WTPChecks { get; set; }
        public DbSet<PhotoFileWTPCheck> PhotoFilesWTPCheck { get; set; }
        public DbSet<GeneratorCheck> GeneratorChecks { get; set; }
        public DbSet<PhotoFileGeneratorCheck> PhotoFilesGeneratorCheck { get; set; }
        public DbSet<SensorsCheck> SensorsChecks { get; set; }
        public DbSet<PhotoFileSensorsCheck> PhotoFilesSensorsCheck { get; set; }
        public DbSet<WaterSamplePlaceType> WaterSamplePlaceTypes { get; set; }
        public DbSet<WaterSampleStandardLimit> StandardLimits { get; set; }
        public DbSet<ConstructionSiteInspection> ConstructionSiteInspections { get; set; }
        public DbSet<PhotoFileConstructionSiteInspection> PhotoFilesConstructionSiteInspection { get; set; }
        public DbSet<Decommission> Decommissions { get; set; }
        public DbSet<PhotoFileDecommission> PhotoFilesDecommission { get; set; }
        public DbSet<HourRegistration> HourRegistrations { get; set; }
        public DbSet<RowHours> RowHours { get; set; }
        public DbSet<HourRegistration_Ongoing> HourRegistrations_Ongoing { get; set; }
        public DbSet<RowHours_Ongoing> RowHours_Ongoing { get; set; }
        public DbSet<SiteCheck> SiteChecks { get; set; }
        public DbSet<PhotoFileSiteCheck> PhotoFilesSiteCheck { get; set; }
        public DbSet<AlarmCall> AlarmCalls { get; set; }
        public DbSet<PhotoFileAlarmCall> PhotoFilesAlarmCall { get; set; }
        public DbSet<ReportType> ReportTypes { get; set; }
        public DbSet<ClearPumpTest> ClearPumpTests { get; set; }
        public DbSet<ClearPumpTestData> ClearPumpTestDatas { get; set; }
        public DbSet<ClearPumpTestPhoto> ClearPumpTestPhotos { get; set; }
        public DbSet<InformationEntry> InformationEntries { get; set; }
        //CGJensen Classes
        public DbSet<Dagsrapport> Dagsrapporter { get; set; }
        public DbSet<DagsRapport_TimeRegistrering> Dags_TimeRegistreringer { get; set; }
        public DbSet<DagsRapport_EkstraArbejde> Dags_Eksterarbejder { get; set; }
        public DbSet<DagsRapport_KontrakArbejde> Dags_Kontraktarbejder { get; set; }
        public DbSet<Structure> Structures { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Junction> Junctions { get; set; }
        public DbSet<RoadSection> RoadSections { get; set; }
        public DbSet<AccomodationWork> AccomodationWorks { get; set; }
        public DbSet<SiteClearancesBetweenContractBorder> SiteClearancesBetweenContractBorders { get; set; }
        public DbSet<SiteClearanceForUtilityRelocation> SiteClearancesForUtilityReolocations { get; set; }
        public DbSet<Protocol> Protocols { get; set; }
        public DbSet<EA> EAs { get; set; }
        //CGJensen Classes SLUT
        public DbSet<TelefonListe> TelefonListen { get; set; }
        public DbSet<MaintenanceEntry> MaintenanceEntries { get; set; }
        public DbSet<HourSchedule> HourSchedules { get; set; }
        public DbSet<DataLoggerInstall> DataLoggerInstallations { get; set; }
        public DbSet<PhotoFileDataLoggerInstall> DataLogggerInstallPhotos { get; set; }
        public DbSet<Well> Wells { get; set; }
        public DbSet<WellLayer> WellLayers { get; set; }
        public DbSet<LayerType> LayerTypes { get; set; }
        public DbSet<PipeCut> PipeCuts { get; set; }
        public DbSet<PipeCutPhoto> PipeCutPhotos { get; set; }
        public DbSet<Layer> Layers { get; set; }
        public DbSet<Filter1Layer> Filter1Layers { get; set; }
        public DbSet<Filter2Layer> Filter2Layers { get; set; }
        public DbSet<WellDrillingInstruction> WellDrillingInstructions { get; set; }
        public DbSet<BentoniteLayer> BentoniteLayers { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        //ProjectStatus
        public DbSet<ProjectStatus> ProjectStatuses { get; set; }
        public DbSet<ProjectCategory> ProjectCategories { get; set; }
        public DbSet<StatusDescription> StatusDescriptions { get; set; }
        //
        public DbSet<CGJensenAdmin> CGJensenAdmins { get; set; }
        public DbSet<HorizontalDrainTest> HorizontalDrainTests { get; set; }
        public DbSet<PhotoFileHorizontalDrainTest> PhotoFileHorizontalDrainTests { get; set; }
        //Tasks
        public DbSet<WorkTask> WorkTasks { get; set; }
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<PhotoFileWorkItem> WorkItemPhotos { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<PhotoFileFeedback> FeedbackPhotos { get; set; }
        //End Tasks
        public DbSet<TruckDailyReport> TruckDailyReports { get; set; }
        public DbSet<TruckSite> TruckSites { get; set; }
        public DbSet<PumpTesting> PumpTestings { get; set; }
        public DbSet<PumptestingData> PumptestingDatas { get; set; }
        public DbSet<Grouting> Groutings { get; set; }
        public DbSet<GroutTestDataDevice> GroutDataDevice { get; set; }
        public DbSet<GroutBeforePhoto> GroutBeforePhotos { get; set; }
        public DbSet<GroutGroutPhoto> GroutGroutPhotos { get; set; }
        public DbSet<GroutAfterPhoto> GroutAfterPhotos { get; set; }
        public DbSet<GroutWMBeforePhoto> GroutWMBeforePhotos { get; set; }
        public DbSet<GroutWMAfterPhoto> GroutWMAfterPhotos { get; set; }

        //WTPBUILDER CLASSES
        public DbSet<FilterMaterial> FilterMaterials { get; set; }
        public DbSet<Contamination> Contaminations { get; set; }
        public DbSet<Effort> Efforts { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<MediaEfficiency> MediaEfficiencies { get; set; }
        public DbSet<Atom> Atoms { get; set; }
        public DbSet<WTP_block> WTP_blocks { get; set; }
        public DbSet<Luxurity> Luxurities { get; set; }
        public DbSet<Water_type> Water_types { get; set; }
        public DbSet<SpecialCase> Special_Cases_Air_Strippers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Temporal_section> Temporal_sections { get; set; }
        public DbSet<Dosing> Dosings { get; set; }
        public DbSet<Effect_type> Effect_types { get; set; }
        public DbSet<WTPUnit> WTPUnits { get; set; }
        //WTPBUILDER CLASSES

        public DbSet<Drill> Drillings { get; set; }
        public DbSet<DrillPhoto> DrillPhotos { get; set; }
        public DbSet<Diet> Diets { get; set; }
        public DbSet<PhotoDocumenation> PhotoDocumenations { get; set; }
        public DbSet<PhotoDoc> PhotoDocs { get; set; }
        public DbSet<WaterSamplePackage> WaterSamplePackages { get; set; }
        public DbSet<WaterSampleTypeWaterSamplePackage> WaterSampleTypeWaterSamplePackages { get; set; }
        public DbSet<Discount_Installation> Discount_Installations { get; set; }
        public DbSet<Item_Location> Item_Locations { get; set; }
        public DbSet<SystemCheck> SystemChecks { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ProjectStatusProjectCategory> ProjectStatusProjectCategories { get; set; }
        public DbSet<SoilSample> SoilSamples { get; set; }
        public DbSet<BentoniteLayerWell> BentoniteWellLayers { get; set; }
        public DbSet<FilterLayerWell> FilterWellLayers { get; set; }
        public DbSet<SandLayerWell> SandWellLayers { get; set; }
        public DbSet<SandType> SandTypes { get; set; }
        public DbSet<CastingType> CastingTypes { get; set; }
        public DbSet<PersonalFile> PersonalFiles { get; set; }
        public DbSet<ItemActivity> ItemActivities { get; set; }
        public DbSet<NeedsOilChange> NeedsOilChanges { get; set; }
        public DbSet<PhotoFileMeas> MeasPhotos { get; set; }
        public DbSet<InvoiceItemDB> SnapShotItems { get; set; }
        public DbSet<InvoiceSnapShot> SnapShots { get; set; }
        public DbSet<WellCheck> WellChecks { get; set; }
        public DbSet<PhotoFileWellCheck> WellCheckPhotos { get; set; }
        public DbSet<PreExcavation> PreExcavations { get; set; }
        public DbSet<PreExcavationBeforePhoto> PreExcavationBeforePhotos { get; set; }
        public DbSet<PreExcavationAfterPhoto> PreExcavationAfterPhotos { get; set; }
        public DbSet<PreExcavationPhoto> PreExcavationPhotos { get; set; }
        public DbSet<AcidTreatment> AcidTreatments { get; set; }
        public DbSet<AcidData> AcidDatas { get; set; }
        public DbSet<WellType> WellTypes { get; set; }
        public DbSet<DecommissionableItem> DecommissionableItems { get; set; }
        public DbSet<Airlift> Airlifts { get; set; }
        public DbSet<AirliftPhoto> AirliftPhotos { get; set; }
        public DbSet<AlarmReportReceiver> AlarmReportReceivers { get; set; }
        public DbSet<DagsRapportBeton> DagsRapporterBeton { get; set; }     
        public DbSet<KontrakArbejdeBeton> KontraktarbejderBeton { get; set; }
        public DbSet<TimeRegistreringBeton> TimeRegistreringerBeton { get; set; }
        public DbSet<TimeRegistreringEkstraBeton> imeRegistreringerEkstraBeton { get; set; }
        public DbSet<PhotoFileBeton> PhotoFilesBeton { get; set; }
        public DbSet<Materiel> Materieller { get; set; }
        public DbSet<MaterielNumber> MaterielAntaller { get; set; }
        public DbSet<DrillWater> DrillWaters { get; set; }
        public DbSet<DrillWaterPhoto> DrillWaterPhotos { get; set; }
        public DbSet<WaterHandling> WaterHandlings { get; set; }
        public DbSet<SedimentationSiteReport> SedimentationSiteReports { get; set; }
        public DbSet<SedimentationSiteReportPhoto> SedimentationSiteReportPhotos { get; set; }
        public DbSet<Daily_Report_2Backup> DailyReportBackups { get; set; }
        public DbSet<MainOps.Models.PumpInstallation> PumpInstallation { get; set; }
        public DbSet<MainOps.Models.ReinfiltrationInstallation> ReinfiltrationInstallation { get; set; }
        public DbSet<MainOps.Models.ObservationInstallation> ObservationInstallation { get; set; }
        public DbSet<StockRentalItem> StockRentalItems { get; set; }
        public DbSet<StockRentalItemPhotoDelivery> StockRentalItemPhotosDelivery { get; set; }
        public DbSet<StockRentalItemPhotoReturn> StockRentalItemPhotosReturn { get; set; }

        // extra work boq setup!
        public DbSet<ExtraWorkBoQ> ExtraWorkBoQs { get; set; }
        public DbSet<ExtraWorkBoQHeader> ExtraWorkBoQHeaders { get; set; }
        public DbSet<ExtraWorkBoQDescription> ExtraWorkBoQDescriptions { get; set; }
        public DbSet<ExtraWorkBoQItem> ExtraWorkBoQItems { get; set; }

        //
    }
}
