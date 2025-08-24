using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
using MainOps.Data;
using MainOps.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MainOps.Controllers
{
    public class DrawingController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public DrawingController(DataContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env):base(context,userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            ImageModel model = new ImageModel();
            using (MagickImage image = new MagickImage(new MagickColor("yellow"), 500, 500))
            {
                DrawableStrokeColor strokeColor = new DrawableStrokeColor(new MagickColor("purple"));
                DrawableStrokeWidth stokeWidth = new DrawableStrokeWidth(5);
                DrawableFillColor fillColor = new DrawableFillColor(MagickColor.FromRgb((byte)200, (byte)255, (byte)255));
                DrawableCircle circle = new DrawableCircle(250, 250, 100, 100);

                image.Draw(strokeColor, stokeWidth, fillColor, circle);
                var path = _env.WebRootPath + "\\DrawingTest\\" +"circl.png";
                image.Write(path);
                model.Image = image;
                model.path = path;
            }

            return View(model);
        }
    }
    public class ImageModel
    {
        public MagickImage Image { get; set; }
        public string path { get; set; }
        public ImageModel(MagickImage m)
        {
            this.Image = m;
        }
        public ImageModel()
        {

        }
    }
}