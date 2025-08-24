using MainOps.Data;
using MainOps.Models.ReportClasses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class MyBackGroundService : BackgroundService
    {
        private AllDocumentation _doc;
        private DataContext _context;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
        private ControllerContext _cont;

        public MyBackGroundService()
        {
            
            
        }
        protected async override Task ExecuteAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (this._cont != null && this._context != null && this._env != null && this._doc != null)
                {
                    await TheTask(_doc);
                    await StopAsync(token);
                }
                //await Task.Delay(1000000, token);
            }
        }
        public void AddDocuementation(AllDocumentation doc,ControllerContext cont,DataContext context, 
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting)
        {
            this._doc = doc;
            this._cont = cont;
            this._context = context;
            this._env = hosting;
        }
        private async Task TheTask(AllDocumentation doc)
        {
            var project = _context.Projects.Find(doc.ProjectId);
            ViewAsPdf pdf = new ViewAsPdf("Reports/AllDocumentation", doc)
            {
                FileName = "DOC_" + project.Abbreviation + "_" + doc.starttime.ToString("yyyy-MM-dd") + "_" + doc.endtime.ToString("yyyy-MM-dd") + ".pdf",
            };
            string fullPath = _env.WebRootPath + "\\AHAK\\akonto\\Documentation\\";
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            byte[] pdfData = await pdf.BuildFile(_cont);
            
            
           using (var fileStream = new FileStream(fullPath + pdf.FileName, FileMode.Create, FileAccess.Write))
           {
                    fileStream.Write(pdfData, 0, pdfData.Length);
           }
           
           
        }
    }
}
