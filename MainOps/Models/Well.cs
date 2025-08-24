using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class WellPdf : Well
    {
        public double heightfactor { get; set; }
        public double heightfactor10th { get { return heightfactor / 10.0; } }
        public double heightfactor100th { get { return heightfactor / 100.0; } }
        public string heightstring
        {
            get
            {
                return String.Concat(heightfactor10th, "cm !important");
            }
        }
        public string heightstring0
        {
            get
            {
                return @String.Format("{0}cm", this.heightfactor);
            }
        }
        public double heightscale
        {
            get
            {
                return this.heightfactor10th;
            }
        }
        public int ilength
        {
            get
            {
                return Convert.ToInt32(this.WellLayers.Select(x => x.End_m).Max() * 100);
            }
        }
        public string tableheight
        {
            get
            {
                return String.Format("{0}cm !important", Convert.ToDecimal(this.ilength / 100.0 * this.heightfactor));
            }
        }
        public string heightoftd
        {
            get
            {
                return String.Format("{0}cm", this.WellLayers.Select(x => x.End_m).Max() * 100 * this.heightscale);
            }
        }
        public int rowspanruler
        {
            get
            {
                return Convert.ToInt32(Convert.ToDouble(this.WellLayers.Select(x => x.End_m).Max()) * 100);
            }
        }
        public WellPdf()
        {

        }
        public WellPdf(Well vm)
        {
            this.Id = vm.Id;
            this.DGU_Number = vm.DGU_Number;
            this.WellDiameter = vm.WellDiameter;
            this.PipeDiameter = vm.PipeDiameter;
            this.Coord_x = vm.Coord_x;
            this.Coord_y = vm.Coord_y;
            this.Coord_z = vm.Coord_z;
            this.DivisionId = vm.DivisionId;
            this.Division = vm.Division;
            this.ProjectId = vm.ProjectId;
            this.SubProjectId = vm.SubProjectId;
            this.WellName = vm.WellName;
            this.Well_Depth = vm.Well_Depth;
            this.Drill_Date_Start = vm.Drill_Date_Start;
            this.Drill_Date_End = vm.Drill_Date_End;
            this.Done_By = vm.Done_By;
            this.Drill_Method = vm.Drill_Method;
            this.CoordSystemId = vm.CoordSystemId;
            this.Attachments = vm.Attachments;
            this.Assessed_By = vm.Assessed_By;
            this.Assessed_Date = vm.Assessed_Date;
            this.Approved_By = vm.Approved_By;
            this.Approved_Date = vm.Approved_Date;
            this.WaterLevel = vm.WaterLevel;
            this.WellTypeId = vm.WellTypeId;
            this.TopOfPipe = vm.TopOfPipe;
            this.WellDrillingInstructionId = vm.WellDrillingInstructionId;
            this.WelLDrillingInstruction = vm.WelLDrillingInstruction;
            this.Well_Type = vm.Well_Type;
            this.BentoniteLayers = vm.BentoniteLayers;
            this.CoordSystem = vm.CoordSystem;
            this.FilterLayers = vm.FilterLayers;
            this.SandLayers = vm.SandLayers;
            this.SoilSamples = vm.SoilSamples;
            this.SubProject = vm.SubProject;
            this.Project = vm.Project;
            this.WellLayers = vm.WellLayers;
        }
    }


    public class Well
    {
        [Key]
        public int Id { get; set; }
        public int? DivisionId { get; set; }
        public virtual Division? Division { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Well Name")]
        public string? WellName { get; set; }
        [Display(Name = "X Coordinate")]
        public double? Coord_x { get; set; }
        [Display(Name = "Y Coordinate")]
        public double? Coord_y { get; set; }
        [Display(Name = "Z Coordinate")]
        public double? Coord_z { get; set; }
        [ForeignKey("CoordSystem")]
        public int? CoordSystemId { get; set; }
        public virtual CoordSystem? CoordSystem { get; set; }
        [Display(Name = "DGU Number")]
        public string? DGU_Number { get; set; }
        [Display(Name = "Depth (m)")]
        public double? Well_Depth { get; set; }
        [Display(Name = "Ground Layers")]
        public ICollection<WellLayer>? WellLayers { get; set; }
        [Display(Name = "Drill Method")]
        public string? Drill_Method { get; set; }
        [Display(Name = "Well Type")]
        public string? Well_Type { get; set; }
        [Display(Name = "Start Date Drilled")]
        public DateTime Drill_Date_Start { get; set; }
        [Display(Name = "End Date Drilled")]
        public DateTime Drill_Date_End { get; set; }
        [Display(Name = "Done By")]
        public string? Done_By { get; set; }
        [Display(Name = "Assessed By")]
        public string? Assessed_By { get; set; }
        [Display(Name = "Assessment Date")]
        public DateTime? Assessed_Date { get; set; }
        [Display(Name = "Approved By")]
        public string? Approved_By { get; set; }
        [Display(Name = "Approvement Date")]
        public DateTime? Approved_Date { get; set; }
        public string? WellDiameter { get; set; }
        public string? PipeDiameter { get; set; }
        public string? Attachments { get; set; }
        public double? WaterLevel { get; set; }
        public double? TopOfPipe { get; set; }

        [ForeignKey("WellDrillingInstruction")]
        public int? WellDrillingInstructionId { get; set; }
        public virtual WellDrillingInstruction? WelLDrillingInstruction { get; set; }
        public ICollection<SoilSample>? SoilSamples { get; set; }
        public ICollection<BentoniteLayerWell>? BentoniteLayers { get; set; }
        public ICollection<FilterLayerWell>? FilterLayers { get; set; }
        public ICollection<SandLayerWell>? SandLayers { get; set; }
        public int? WellTypeId { get; set; }
        public virtual WellType? WellType { get; set; }
        //public string BoreProfileRightText { get; set; }
        public Well()
        {
        }
        public Well(WellVM vm, ApplicationUser user)
        {
            this.DGU_Number = vm.DGU;
            this.WellDiameter = vm.WellDiameter;
            this.PipeDiameter = vm.PipeDiameter;
            this.Coord_x = vm.Coord_x;
            this.Coord_y = vm.Coord_y;
            this.Coord_z = vm.Coord_z;
            this.DivisionId = user.DivisionId;
            this.ProjectId = vm.ProjectId;
            this.SubProjectId = vm.SubProjectId;
            this.WellName = vm.WellName;
            this.Well_Depth = vm.Layers.Last().End_m;
            this.Drill_Date_Start = vm.Drill_Date_Start;
            this.Drill_Date_End = vm.Drill_Date_End;
            this.Done_By = user.full_name();
            this.Drill_Method = vm.DrillMethod;
            this.CoordSystemId = vm.CoordSystemId;
            this.Attachments = vm.Attachments;
            this.Assessed_By = vm.Assessed_By;
            this.Assessed_Date = vm.Assessed_Date;
            this.Approved_By = vm.Approved_By;
            this.Approved_Date = vm.Approved_Date;
            this.WaterLevel = vm.WaterLevel;
            this.WellTypeId = vm.WellTypeId;
            this.TopOfPipe = vm.TopOfPipe;

        }
    }
    public class WellFooterInfo
    {
        public string? welltype { get; set; }
        public string? Pipe1DN { get; set; }
        public string? Pipe1Slots { get; set; }
        public string? Pipe1Top { get; set; }
        public string? Method { get; set; }
        public string? coordsystem { get; set; }
        public string? coordx { get; set; }
        public string? coordy { get; set; }
        public string? Attachments { get; set; }
        public string? project { get; set; }
        public string? projectnr { get; set; }
        public string? subprojectName { get; set; }
        public string? drilled_by { get; set; }
        public string? date { get; set; }
        public string? assessedby { get; set; }
        public string? dgunumber { get; set; }
        public string? Wellname { get; set; }
        public string? doneby { get; set; }
        public string? approvedby { get; set; }
        public string? approveddate { get; set; }
        public double? WaterLevel { get; set; }
        public WellFooterInfo()
        {

        }
        public WellFooterInfo(Well model)
        {
            this.welltype = model.Well_Type;
            this.Pipe1DN = model.PipeDiameter;
            this.Pipe1Top = String.Format("{0}mDVR90", model.TopOfPipe);
            if (model.FilterLayers.Count > 0)
            {
                this.Pipe1Slots = model.FilterLayers.First().Slitsize;
            }
            else
            {
                this.Pipe1Slots = "";
            }
            if (model.Coord_z != null)
            {
                this.Pipe1Top = Convert.ToDouble(model.Coord_z).ToString("N3");
            }
            this.Method = model.Drill_Method;
            this.coordsystem = model.CoordSystem.system;
            if (model.Coord_x != null)
            {
                this.coordx = Convert.ToDouble(model.Coord_x).ToString("N3");
            }
            if (model.Coord_y != null)
            {
                this.coordy = Convert.ToDouble(model.Coord_y).ToString("N3");
            }
            this.projectnr = model.Project.ProjectNr;
            this.project = model.Project.Name;
            if (model.SubProjectId != null)
            {
                this.subprojectName = model.SubProject.Name;
            }
            this.WaterLevel = model.WaterLevel;
            this.drilled_by = model.Division.Name;
            this.date = model.Drill_Date_End.ToString();
            this.assessedby = model.Assessed_By;
            this.dgunumber = model.DGU_Number;
            this.Wellname = model.WellName;
            this.doneby = model.Done_By;
            this.approvedby = model.Approved_By;
            this.approveddate = model.Approved_Date.ToString();
            if (this.welltype == null)
            {
                this.welltype = "";
            }
            if (this.Pipe1DN == null)
            {
                this.Pipe1DN = "";
            }
            if (this.Method == null)
            {
                this.Method = "";
            }
            if (this.coordsystem == null)
            {
                this.coordsystem = "";
            }
            if (this.coordx == null)
            {
                this.coordx = "";
            }
            if (this.coordy == null)
            {
                this.coordy = "";
            }
            if (this.assessedby == null)
            {
                this.assessedby = "";
            }
            if (this.approvedby == null)
            {
                this.approvedby = "";
            }
            if (this.approveddate == null)
            {
                this.approveddate = "";
            }
            if (this.date == null)
            {
                this.date = "";
            }
            if (this.dgunumber == null)
            {
                this.dgunumber = "";
            }
        }

    }
    public class WellVM
    {
        public int WellId { get; set; }
        public string? DGU { get; set; }
        public List<WellLayer> Layers { get; set; }
        public List<SoilSample> Samples { get; set; }
        public List<BentoniteLayerWell> BentoniteLayers { get; set; }
        public List<FilterLayerWell> FilterLayers { get; set; }
        public List<SandLayerWell> SandLayers { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public string? Assessed_By { get; set; }
        [Display(Name = "Assessment Date")]
        public DateTime? Assessed_Date { get; set; }
        [Display(Name = "Approved By")]
        public string? Approved_By { get; set; }
        [Display(Name = "Approvement Date")]
        public DateTime? Approved_Date { get; set; }
        public string? WellDiameter { get; set; }
        public string? PipeDiameter { get; set; }
        public int? SubProjectId { get; set; }
        public string? Attachments { get; set; }
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Well Name")]
        public string? WellName { get; set; }
        [Display(Name = "X Coordinate")]
        public double? Coord_x { get; set; }
        [Display(Name = "Y Coordinate")]
        public double? Coord_y { get; set; }
        [Display(Name = "Z Coordinate")]
        public double? Coord_z { get; set; }
        [Display(Name = "Drill Method")]
        public string? DrillMethod { get; set; }
        [Display(Name = "Start Date Drilled")]
        public DateTime Drill_Date_Start { get; set; }
        [Display(Name = "End Date Drilled")]
        public DateTime Drill_Date_End { get; set; }
        [Display(Name = "Done By")]
        public string? Done_By { get; set; }
        public int? CoordSystemId { get; set; }
        public virtual CoordSystem? CoordSystem { get; set; }
        public int? WellTypeId { get; set; }
        public virtual WellType? WellType { get; set; }
        public double? WaterLevel { get; set; }
        public double? TopOfPipe { get; set; }
        public WellVM()
        {
            Layers = new List<WellLayer>(12);
            Samples = new List<SoilSample>(50);
            FilterLayers = new List<FilterLayerWell>(5);
            SandLayers = new List<SandLayerWell>(7);
            BentoniteLayers = new List<BentoniteLayerWell>(7);
            for (int i = 0; i < 12; i++)
            {
                Layers.Add(new WellLayer());
            }
            for (int i = 0; i < 60; i++)
            {
                Samples.Add(new SoilSample());
            }
            for (int i = 0; i < 5; i++)
            {
                FilterLayers.Add(new FilterLayerWell());
            }
            for (int i = 0; i < 7; i++)
            {
                BentoniteLayers.Add(new BentoniteLayerWell());
            }
            for (int i = 0; i < 7; i++)
            {
                SandLayers.Add(new SandLayerWell());
            }
            CoordSystemId = 2;
            Drill_Date_Start = DateTime.Now.Date;
            Drill_Date_End = DateTime.Now.Date;
        }
        public WellVM(List<WellLayer> existing_layers)
        {
            Layers = new List<WellLayer>(12);
            Samples = new List<SoilSample>(60);
            FilterLayers = new List<FilterLayerWell>(5);
            SandLayers = new List<SandLayerWell>(7);
            BentoniteLayers = new List<BentoniteLayerWell>(7);
            for (int i = 0; i < 12; i++)
            {
                if (existing_layers.Count > i)
                {
                    Layers.Add(existing_layers.ElementAt(i));
                }
                else
                {
                    Layers.Add(new WellLayer());
                }
            }
            CoordSystemId = 2;
            Drill_Date_Start = DateTime.Now.Date;
            Drill_Date_End = DateTime.Now.Date;
        }
        public WellVM(List<WellLayer> existing_layers, List<SoilSample> existing_samples, List<FilterLayerWell> existing_filters, List<BentoniteLayerWell> existing_bentonite, List<SandLayerWell> existing_sand)
        {
            Layers = new List<WellLayer>(12);
            Samples = new List<SoilSample>(50);
            FilterLayers = new List<FilterLayerWell>(5);
            SandLayers = new List<SandLayerWell>(7);
            BentoniteLayers = new List<BentoniteLayerWell>(7);
            for (int i = 0; i < 12; i++)
            {
                if (existing_layers.Count > i)
                {
                    Layers.Add(existing_layers.ElementAt(i));
                }
                else
                {
                    Layers.Add(new WellLayer());
                }
            }
            double samplemeter = Convert.ToDouble(existing_samples.Last().sample_meter + 1.0);
            for (int i = 0; i < 60; i++)
            {
                if (existing_samples.Count > i)
                {
                    Samples.Add(existing_samples.ElementAt(i));
                }
                else
                {
                    Samples.Add(new SoilSample { sample_meter = samplemeter });
                    samplemeter += 1.0;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                if (existing_filters.Count > i)
                {
                    FilterLayers.Add(existing_filters.ElementAt(i));
                }
                else
                {
                    FilterLayers.Add(new FilterLayerWell());
                }
            }
            for (int i = 0; i < 7; i++)
            {
                if (existing_bentonite.Count > i)
                {
                    BentoniteLayers.Add(existing_bentonite.ElementAt(i));
                }
                else
                {
                    BentoniteLayers.Add(new BentoniteLayerWell());
                }
            }
            for (int i = 0; i < 7; i++)
            {
                if (existing_sand.Count > i)
                {
                    SandLayers.Add(existing_sand.ElementAt(i));
                }
                else
                {
                    SandLayers.Add(new SandLayerWell());
                }
            }
            CoordSystemId = 2;
            Drill_Date_Start = DateTime.Now.Date;
            Drill_Date_End = DateTime.Now.Date;
        }
        public WellVM(Well model)
        {
            this.WellId = model.Id;
            this.WaterLevel = model.WaterLevel;
            this.TopOfPipe = model.TopOfPipe;
            this.WellTypeId = model.WellTypeId;
            if (model.WellType != null)
            {
                this.WellType = model.WellType;
            }
            this.DGU = model.DGU_Number;
            this.DrillMethod = model.Drill_Method;
            this.WellDiameter = model.WellDiameter;
            this.PipeDiameter = model.PipeDiameter;
            this.Done_By = model.Done_By;
            this.Coord_x = model.Coord_x;
            this.Coord_y = model.Coord_y;
            this.Coord_z = model.Coord_z;
            this.CoordSystemId = model.CoordSystemId;
            this.Drill_Date_Start = model.Drill_Date_Start;
            this.Drill_Date_End = model.Drill_Date_End;
            this.ProjectId = model.ProjectId;
            this.SubProjectId = model.SubProjectId;
            this.WellName = model.WellName;
            this.Layers = new List<WellLayer>(12);
            this.Samples = new List<SoilSample>(50);
            this.FilterLayers = new List<FilterLayerWell>(5);
            this.SandLayers = new List<SandLayerWell>(7);
            this.BentoniteLayers = new List<BentoniteLayerWell>(7);
            this.Attachments = model.Attachments;
            this.Done_By = model.Done_By;
            if (model.WellLayers != null)
            {
                foreach (var item in model.WellLayers)
                {
                    this.Layers.Add(item);
                }
                for (int i = model.WellLayers.Count; i < 12; i++)
                {
                    this.Layers.Add(new WellLayer());
                }
            }
            else
            {
                for (int i = 0; i < 12; i++)
                {
                    this.Layers.Add(new WellLayer());
                }
            }
            if (model.SandLayers != null)
            {
                foreach (var item in model.SandLayers)
                {
                    this.SandLayers.Add(item);
                }
                for (int i = model.SandLayers.Count; i < 7; i++)
                {
                    this.SandLayers.Add(new SandLayerWell());
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    this.SandLayers.Add(new SandLayerWell());
                }
            }
            if (model.SoilSamples != null)
            {
                foreach (var item in model.SoilSamples)
                {
                    this.Samples.Add(item);
                }
                for (int i = model.SoilSamples.Count; i < 60; i++)
                {
                    this.Samples.Add(new SoilSample());
                }
            }
            else
            {
                for (int i = 0; i < 60; i++)
                {
                    this.Samples.Add(new SoilSample());
                }
            }
            if (model.BentoniteLayers != null)
            {
                foreach (var item in model.BentoniteLayers)
                {
                    this.BentoniteLayers.Add(item);
                }
                for (int i = model.BentoniteLayers.Count; i < 7; i++)
                {
                    this.BentoniteLayers.Add(new BentoniteLayerWell());
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    this.BentoniteLayers.Add(new BentoniteLayerWell());
                }
            }
            if (model.FilterLayers != null)
            {
                foreach (var item in model.FilterLayers)
                {
                    this.FilterLayers.Add(item);
                }
                for (int i = model.FilterLayers.Count; i < 5; i++)
                {
                    this.FilterLayers.Add(new FilterLayerWell());
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    this.FilterLayers.Add(new FilterLayerWell());
                }
            }

        }
    }
    public class SandType
    {
        [Key]
        public int Id { get; set; }
        public string? TypeOfSand { get; set; }
    }
    public class CastingType
    {
        [Key]
        public int Id { get; set; }
        public string? TypeOfCasting { get; set; }
    }
    public class SoilSample
    {
        [Key]
        public int Id { get; set; }
        public string? Code { get; set; }
        public double? sample_meter { get; set; }
        public string? Odour { get; set; }
        public bool IsWet { get; set; }
        public string? SoilColor { get; set; }
        public int? WellId { get; set; }
        public virtual Well? Well { get; set; }
    }
    public class WellLayer
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public double Start_m { get; set; }
        public double End_m { get; set; }
        public int LayerId { get; set; }
        public virtual Layer? Layer { get; set; }
        [ForeignKey("Well")]
        public int WellId { get; set; }
        public virtual Well? Well { get; set; }
        public int Nr { get; set; }

        public WellLayer()
        {

        }
    }

    public class Layer
    {
        public int Id { get; set; }
        public string? LayerType { get; set; }
        public string? BackgroundUrl { get; set; }
        public string? TextColor { get; set; }
    }
    public class LayerType
    {
        public int Id { get; set; }
        public int? WellDrillingInstructionId { get; set; }
        public virtual WellDrillingInstruction? WellDrillingInstruction { get; set; }
        public int LayerNumber { get; set; }
        public int? LayerId { get; set; }
        public virtual Layer? Layer { get; set; }
        public decimal Percent { get; set; }
    }
    public class Filter1Layer
    {
        public int Id { get; set; }
        public int? WellDrillingInstructionId { get; set; }
        public virtual WellDrillingInstruction? WellDrillingInstruction { get; set; }
        public double Under { get; set; }
        public int? LayerTypeUnderId { get; set; }
        public double Over { get; set; }
        public int? LayerTypeOverId { get; set; }
        public double Length { get; set; }
        public int? CellStart { get; set; }
        public int? CellEnd { get; set; }
    }
    public class Filter2Layer
    {
        public int Id { get; set; }
        public int? WellDrillingInstructionId { get; set; }
        public virtual WellDrillingInstruction? WellDrillingInstruction { get; set; }
        public double Under { get; set; }
        public int? LayerTypeUnderId { get; set; }
        public double Over { get; set; }
        public int? LayerTypeOverId { get; set; }
        public double Length { get; set; }
        public int? CellStart { get; set; }
        public int? CellEnd { get; set; }
    }
    public class BentoniteLayer
    {
        public int Id { get; set; }
        public int? WellDrillingInstructionId { get; set; }
        public virtual WellDrillingInstruction? WellDrillingInstruction { get; set; }
        public double Under { get; set; }
        public int? LayerTypeUnderId { get; set; }
        public double Over { get; set; }
        public int? LayerTypeOverId { get; set; }
        public int? CellStart { get; set; }
        public int? CellEnd { get; set; }
    }
    public class BentoniteLayerWell
    {
        public int Id { get; set; }
        public int? WellId { get; set; }
        public virtual Well? Well { get; set; }
        public double meter_start { get; set; }
        public double meter_end { get; set; }
        public int? CastingTypeId { get; set; }
        public virtual CastingType? CastingType { get; set; }
    }
    public class FilterLayerWell
    {
        public int Id { get; set; }
        public int? WellId { get; set; }
        public virtual Well? Well { get; set; }
        public double meter_start { get; set; }
        public double meter_end { get; set; }
        public string? Slitsize { get; set; }

        public FilterLayerWell() { }
    }
    public class SandLayerWell
    {
        public int Id { get; set; }
        public int? WellId { get; set; }
        public virtual Well? Well { get; set; }
        public double meter_start { get; set; }
        public double meter_end { get; set; }
        public int? SandTypeId { get; set; }
        public virtual SandType? SandType { get; set; }
    }
    public class WellDrillingInstructionVM
    {
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Display(Name = "Date")]
        public DateTime? Date { get; set; }
        [Display(Name = "Address")]
        public string? Address { get; set; }
        [Display(Name = "Dril Place")]
        public string? DrillPlace { get; set; }
        [Display(Name = "Well ID")]
        public string? WellID { get; set; }
        [Display(Name = "Drill rig")]
        public string? Borerig { get; set; }
        [Display(Name = "Well type")]
        public string? Boringstype { get; set; }
        [Display(Name = "Drilling Method")]
        public string? Boremetode { get; set; }
        [Display(Name = "Coating")]
        public string? Belaegning { get; set; }
        [Display(Name = "Drilling permit?")]
        public bool Boretilladelse { get; set; }
        [Display(Name = "Drilling permit file")]
        public string? Boretilladelse_Path { get; set; }
        [Display(Name = "Discharge permit?")]
        public bool Afledningstilladelse { get; set; }
        [Display(Name = "Discharge permit file")]
        public string? Aflednignstilladelse_Path { get; set; }
        [Display(Name = "Road ownership permit?")]
        public bool RaadenOverVej { get; set; }
        [Display(Name = "Road ownership permit file")]
        public string? RaadenOverVej_Path { get; set; }
        [Display(Name = "Digging permit?")]
        public bool Gravetilladelse { get; set; }
        [Display(Name = "Digging permit file")]
        public string? Gravetilladelse_Path { get; set; }
        [Display(Name = "Wire information(LER) retrieved?")]
        public bool Ledningsoplysninger { get; set; }
        [Display(Name = "Wire information(LER) file")]
        public string? Ledningsoplysninger_Path { get; set; }
        [Display(Name = "Pre-digging")]
        public string? Forgravning { get; set; }
        [Display(Name = "Re-establshment")]
        public string? Reetablering { get; set; }
        [Display(Name = "Two filters?")]
        public bool ToFiltre { get; set; }
        [Display(Name = "Filter 1 dimension")]
        public string? Filter1_Dim { get; set; }
        [Display(Name = "Filter 2 dimension")]
        public string? Filter2_Dim { get; set; }
        [Display(Name = "Blind pipe 1 dimension")]
        public string? Blindroer1 { get; set; }
        [Display(Name = "Blind pipe 2 dimension")]
        public string? Blindroer2 { get; set; }
        [Display(Name = "Filter pipe 1 dimension")]
        public string? Filterroer1 { get; set; }
        [Display(Name = "Filter pipe 2 dimension")]
        public string? Filterroer2 { get; set; }
        [Display(Name = "Filter slots")]
        public string? Filterslidser { get; set; }
        [Display(Name = "Gravel type")]
        public string? Grustype { get; set; }
        [Display(Name = "Sealing rings")]
        public bool Taetningsringe { get; set; }
        [Display(Name = "Filter control")]
        public bool Filterstyr { get; set; }
        [Display(Name = "Sealing")]
        public string? Forsegling { get; set; }
        [Display(Name = "Registration during drilling")]
        public string? Registrering { get; set; }
        [Display(Name = "Sample frequency")]
        public double ProeveFrekvens { get; set; }
        [Display(Name = "Drill depth")]
        public double BoreDybde { get; set; }
        [Display(Name = "Blind pipe in p1?")]
        public bool BlindPipe1 { get; set; }
        [Display(Name = "Blind pipe in p2?")]
        public bool BlindPipe2 { get; set; }
        [Display(Name = "Blind pipe 1 meter")]
        public double BlindPipe1m { get; set; }
        [Display(Name = "Blind pipe 2 meter")]
        public double BlindPipe2m { get; set; }
        [Display(Name = "Layers")]
        public List<LayerType>? Layers { get; set; }
        [Display(Name = "Filter placements p1")]
        public List<Filter1Layer>? Filter1_Layers { get; set; }
        [Display(Name = "Filter placements p2")]
        public List<Filter2Layer>? Filter2_Layers { get; set; }
        [Display(Name = "Bentonite layers")]
        public List<BentoniteLayer>? BentoniteLayers { get; set; }
        [Display(Name = "Filter 1 length")]
        public double Filter1_Laengde { get; set; }
        [Display(Name = "Filter 2 length")]
        public double? Filter2_Laengde { get; set; }
        public WellDrillingInstructionVM()
        {

        }
    }
    public class WellDrillingInstruction
    {
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Display(Name = "Date")]
        public DateTime? Date { get; set; }
        [Display(Name = "Address")]
        public string? Address { get; set; }
        [Display(Name = "Dril Place")]
        public string? DrillPlace { get; set; }
        [Display(Name = "Well ID")]
        public string? WellID { get; set; }
        [Display(Name = "Drill rig")]
        public string? Borerig { get; set; }
        [Display(Name = "Drill type")]
        public string? Boringstype { get; set; }
        [Display(Name = "Drilling Method")]
        public string? Boremetode { get; set; }
        [Display(Name = "Coating")]
        public string? Belaegning { get; set; }
        [Display(Name = "Drilling permit?")]
        public bool Boretilladelse { get; set; }
        [Display(Name = "Drilling permit file")]
        public string? Boretilladelse_Path { get; set; }
        [Display(Name = "Discharge permit?")]
        public bool Afledningstilladelse { get; set; }
        [Display(Name = "Discharge permit file")]
        public string? Aflednignstilladelse_Path { get; set; }
        [Display(Name = "Road ownership permit?")]
        public bool RaadenOverVej { get; set; }
        [Display(Name = "Road ownership permit file")]
        public string? RaadenOverVej_Path { get; set; }
        [Display(Name = "Digging permit?")]
        public bool Gravetilladelse { get; set; }
        [Display(Name = "Digging permit file")]
        public string? Gravetilladelse_Path { get; set; }
        [Display(Name = "Wire information(LER) retrieved?")]
        public bool Ledningsoplysninger { get; set; }
        [Display(Name = "Wire information(LER) file")]
        public string? Ledningsoplysninger_Path { get; set; }
        [Display(Name = "Pre-digging")]
        public string? Forgravning { get; set; }
        [Display(Name = "Re-establshment")]
        public string? Reetablering { get; set; }
        [Display(Name = "Two filters?")]
        public bool ToFiltre { get; set; }
        [Display(Name = "Filter 1 dimension")]
        public string? Filter1_Dim { get; set; }
        [Display(Name = "Filter 2 dimension")]
        public string? Filter2_Dim { get; set; }
        [Display(Name = "Blind pipe 1 dimension")]
        public string? Blindroer1 { get; set; }
        [Display(Name = "Blind pipe 2 dimension")]
        public string? Blindroer2 { get; set; }
        [Display(Name = "Filter pipe 1 dimension")]
        public string? Filterroer1 { get; set; }
        [Display(Name = "Filter pipe 2 dimension")]
        public string? Filterroer2 { get; set; }
        [Display(Name = "Filter slots")]
        public string? Filterslidser { get; set; }
        [Display(Name = "Gravel type")]
        public string? Grustype { get; set; }
        [Display(Name = "Sealing rings")]
        public bool Taetningsringe { get; set; }
        [Display(Name = "Filter control")]
        public bool Filterstyr { get; set; }
        [Display(Name = "Sealing")]
        public string? Forsegling { get; set; }
        [Display(Name = "Registration during drilling")]
        public string? Registrering { get; set; }
        [Display(Name = "Sample frequency")]
        public double ProeveFrekvens { get; set; }
        [Display(Name = "Drill depth")]
        public double BoreDybde { get; set; }
        [Display(Name = "Blind pipe in p1?")]
        public bool BlindPipe1 { get; set; }
        [Display(Name = "Blind pipe in p2?")]
        public bool BlindPipe2 { get; set; }
        [Display(Name = "Blind pipe 1 meter")]
        public double BlindPipe1m { get; set; }
        [Display(Name = "Blind pipe 2 meter")]
        public double BlindPipe2m { get; set; }
        [Display(Name = "Layers")]
        public ICollection<LayerType>? Layers { get; set; }
        [Display(Name = "Filter placements p1")]
        public ICollection<Filter1Layer>? Filter1_Layers { get; set; }
        [Display(Name = "Filter placements p2")]
        public ICollection<Filter2Layer>? Filter2_Layers { get; set; }
        [Display(Name = "Bentonite layers")]
        public ICollection<BentoniteLayer>? BentoniteLayers { get; set; }
        [Display(Name = "Filter 1 length")]
        public double Filter1_Laengde { get; set; }
        [Display(Name = "Filter 2 length")]
        public double? Filter2_Laengde { get; set; }
        [Display(Name = "Done By")]
        public string? DoneBy { get; set; }
        public WellDrillingInstruction()
        {

        }
        public WellDrillingInstruction(WellDrillingInstructionVM modelIn)
        {
            this.Aflednignstilladelse_Path = modelIn.Aflednignstilladelse_Path;
            this.Afledningstilladelse = modelIn.Afledningstilladelse;
            this.Belaegning = modelIn.Belaegning;
            this.BlindPipe1 = modelIn.BlindPipe1;
            this.BlindPipe1m = modelIn.BlindPipe1m;
            this.BlindPipe2 = modelIn.BlindPipe2;
            this.BlindPipe2m = modelIn.BlindPipe2m;
            this.Blindroer1 = modelIn.Blindroer1;
            this.Blindroer2 = modelIn.Blindroer2;
            this.BoreDybde = modelIn.BoreDybde;
            this.Boremetode = modelIn.Boremetode;
            this.Borerig = modelIn.Borerig;
            this.Boringstype = modelIn.Boringstype;
            this.Filter1_Dim = modelIn.Filter1_Dim;
            this.Filter1_Laengde = modelIn.Filter1_Laengde;
            this.Filter2_Dim = modelIn.Filter2_Dim;
            this.Filterroer1 = modelIn.Filterroer1;
            this.Filterroer2 = modelIn.Filterroer2;
            this.Filterslidser = modelIn.Filterslidser;
            this.Filterstyr = modelIn.Filterstyr;
            this.Forgravning = modelIn.Forgravning;
            this.Forsegling = modelIn.Forsegling;
            this.Gravetilladelse = modelIn.Gravetilladelse;
            this.Gravetilladelse_Path = modelIn.Gravetilladelse_Path;
            this.Grustype = modelIn.Grustype;
            this.Ledningsoplysninger = modelIn.Ledningsoplysninger;
            this.Ledningsoplysninger_Path = modelIn.Ledningsoplysninger_Path;
            this.ProeveFrekvens = modelIn.ProeveFrekvens;
            this.ProjectId = modelIn.ProjectId;
            this.RaadenOverVej = modelIn.RaadenOverVej;
            this.RaadenOverVej_Path = modelIn.RaadenOverVej_Path;
            this.Reetablering = modelIn.Reetablering;
            this.Registrering = modelIn.Registrering;
            this.Taetningsringe = modelIn.Taetningsringe;
            this.ToFiltre = modelIn.ToFiltre;
            this.Date = modelIn.Date;
            this.WellID = modelIn.WellID;
            this.DrillPlace = modelIn.DrillPlace;
            this.Address = modelIn.Address;
        }
    }
}
