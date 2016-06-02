using AspNetCoreFileManager.Models;
using ImageProcessorCore;
using ImageProcessorCore.Samplers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;
using System.IO;

namespace AspNetCoreFileManager.Web.Controllers
{
    [Route("image-editing")]
    public class ImageEditingController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ImageEditingController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("resize-image")]
        public IActionResult ResizeImage()
        {
            return View(new ResizeImageUploadViewModel());
        }

        [Route("resize-image")]
        [HttpPost]
        public IActionResult ResizeImage(ResizeImageUploadViewModel resizeImageUpload)
        {
            if (!ModelState.IsValid || resizeImageUpload.FormFile.Length == 0)
                return View(resizeImageUpload);

            var uploadDir = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            var fileName = ContentDispositionHeaderValue.Parse(resizeImageUpload.FormFile.ContentDisposition).FileName.Trim('"');
            fileName = Path.GetFileNameWithoutExtension(fileName).Slugify() + Path.GetExtension(fileName);
            var filePath = Path.Combine(uploadDir, fileName);

            var sw = new Stopwatch();
            sw.Start();
            using (var uploadStream = new MemoryStream())
            {
                resizeImageUpload.FormFile.CopyTo(uploadStream);
                using (var outputStream = System.IO.File.OpenWrite(filePath))
                using (var image = new Image(uploadStream))
                {
                    image.Resize(resizeImageUpload.Width, resizeImageUpload.Height).Save(outputStream);
                }
                sw.Stop();
                ViewBag.ProcessingTime = sw.Elapsed.TotalSeconds.ToString();
            }

            ViewBag.UploadedFilePath = "/uploads/" + fileName;

            return View(resizeImageUpload);
        }
    }
}