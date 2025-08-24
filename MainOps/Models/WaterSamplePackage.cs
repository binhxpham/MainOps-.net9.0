using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class WaterSamplePackage
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "Sample Name")]
        public string Annotation { get; set; }
        [Display(Name = "Content of Sample")]
        public string ListOfComponents { get; set; }
        public ICollection<WaterSampleTypeWaterSamplePackage> WaterSampleTypeWaterSamplePackages { get; set; }
        public WaterSamplePackage()
        {

        }
        public WaterSamplePackage(int? ProjectId,string Annotation,string ListOfComponents)
        {
            this.ProjectId = ProjectId;
            this.Annotation = Annotation;
            this.ListOfComponents = ListOfComponents;
        }
    }
    public class WaterSamplePackagePDFViewModel
    {
        public List<WaterSamplePackage> packages;
        public List<string> result;
        public WaterSamplePackagePDFViewModel(List<WaterSamplePackage> packages,List<string> result)
        {
            this.packages = packages;
            this.result = result;
        }
        public WaterSamplePackagePDFViewModel()
        {

        }
    }
    public class WaterSampleType
    {
        public int Id { get; set; }
        public string Komponent { get; set; }
        public string Enhed { get; set; }
        public double DL { get; set; }
        public ICollection<WaterSampleTypeWaterSamplePackage> WaterSampleTypeWaterSamplePackages { get; set; }
    }
    public class WaterSampleTypeWaterSamplePackage
    {
        public int WaterSamplePackageId { get; set; }
        public virtual WaterSamplePackage WaterSamplePackage { get; set; }
        public int WaterSampleTypeId { get; set; }
        public virtual WaterSampleType WaterSampleType { get; set; }
        public WaterSampleTypeWaterSamplePackage(WaterSamplePackage WSP, WaterSampleType WST)
        {
            this.WaterSamplePackageId = WSP.Id;
            this.WaterSampleTypeId = WST.Id;
        }
        public WaterSampleTypeWaterSamplePackage()
        {

        }
    }
    public class UploadPackageViewModel
    {
        public int? ProjectId { get; set; }
        public string Annotation { get; set; }
    }
    public class CreatePackageViewModel
    {
        public int? ProjectId { get; set; }
        public string Annotation { get; set; }
        public string WaterSampleTypes { get; set; }
        public string WaterSampleTypesNames { get; set; }
        public int? OldPackageId { get; set; }
        public CreatePackageViewModel()
        {

        }
        public CreatePackageViewModel(WaterSamplePackage modelin, List<WaterSampleTypeWaterSamplePackage> itemlist)
        {
            this.OldPackageId = modelin.Id;
            this.ProjectId = modelin.ProjectId;
            this.Annotation = modelin.Annotation;
            if(itemlist.Count > 0)
            {
                this.WaterSampleTypesNames = itemlist.First().WaterSampleType.Komponent;
                this.WaterSampleTypes = itemlist.First().WaterSampleTypeId.ToString();
            }
            for(int i = 1; i<itemlist.Count();i++)
            {
                this.WaterSampleTypesNames += "," + itemlist[i].WaterSampleType.Komponent;
                this.WaterSampleTypes += "," + itemlist[i].WaterSampleTypeId.ToString();
            }
        }
    }
}
