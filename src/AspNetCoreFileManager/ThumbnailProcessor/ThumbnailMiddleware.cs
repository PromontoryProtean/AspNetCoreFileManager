using ImageProcessorCore;
using ImageProcessorCore.Samplers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreFileManager.ThumbnailProcessor
{
    public class ThumbnailProcessorMiddleware
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly RequestDelegate _next;

        public ThumbnailProcessorMiddleware(RequestDelegate next, IHostingEnvironment hostingEnvironment)
        {
            _next = next;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!IsImageRequest(context))
            {
                // move to the next request here?
                await _next.Invoke(context);
                return;
            }

            try
            {
                //var sb = new StringBuilder();
                //sb.AppendLine("=====================================================================================================");
                //sb.AppendLine("Date/Time: " + DateTime.UtcNow.ToString("M/d/yyyy hh:mm tt"));

                int width = 0;
                int.TryParse(context.Request.Query["width"], out width);
                int height = 0;
                int.TryParse(context.Request.Query["height"], out height);

                var inputPath = _hostingEnvironment.ContentRootPath + "/wwwroot" + context.Request.Path;
                //sb.AppendLine("Input Path: " + inputPath);
                //sb.AppendLine("Querystring Dimensions: " + width.ToString() + "x" + height.ToString());

                using (var inputStream = File.OpenRead(inputPath))
                using (var outputStream = new MemoryStream())
                using (var image = new Image(inputStream))
                {
                    if (image.Width > image.Height && width != 0)
                        height = (125 * image.Height) / image.Width;
                    if (image.Height > image.Width && height != 0)
                        width = (125 * image.Width) / image.Height;

                    //sb.AppendLine("Original Dimensions: " + image.Width.ToString() + "x" + image.Height.ToString());
                    //sb.AppendLine("Output Dimensions: " + width.ToString() + "x" + height.ToString());
                    //sb.AppendLine("Image Read Time in Seconds: " + sw.Elapsed.TotalSeconds.ToString());

                    // write directly to the body output stream

                    //var sw = new Stopwatch();
                    //sw.Start();
                    image.Resize(width, height)
                        .Save(outputStream);
                    var bytes = outputStream.ToArray();
                    //sw.Stop();
                    //sb.AppendLine("Image Processing Time in Seconds: " + sw.Elapsed.TotalSeconds.ToString());

                    //context.Response.Body.Write(bytes, 0, bytes.Length);
                    await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                }

                //// the following approach will write to disk then serve the image
                ////var inputPath = _hostingEnvironment.ContentRootPath + "/wwwroot" + context.Request.Path;
                //var outputPath = _hostingEnvironment.ContentRootPath + "/Uploads/" + Path.GetFileName(context.Request.Path);
                //using (var outputStream = File.OpenWrite(outputPath))
                //{
                //    image.Resize(width, height)
                //        .Save(outputStream);
                //}
                //var bytes = File.ReadAllBytes(outputPath);
                //await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);

                //var logFilePath = _hostingEnvironment.ContentRootPath + "/Logs/ThumbnailLog-" + DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm-ss") + ".txt";
                //var logFilePath = _hostingEnvironment.ContentRootPath + "/Logs/ThumbnailLog.txt";
                //var logWriter = new LogWriter(logFilePath);
                //logWriter.Write(sb.ToString());
            }
            catch (Exception ex)
            {
                var logFilePath = _hostingEnvironment.ContentRootPath + "/Logs/Exceptions.txt";
                var logWriter = new LogWriter(logFilePath);
                logWriter.WriteLine("=====================================================================================================");
                logWriter.WriteLine(ex.ToString());
            }

            //await _next.Invoke(context);
        }

        public bool IsImageRequest(HttpContext context)
        {
            if (!IsImageExtension(Path.GetExtension(context.Request.Path.Value).ToLower()))
                return false;

            if (string.IsNullOrWhiteSpace(context.Request.Query["width"]) && string.IsNullOrWhiteSpace(context.Request.Query["height"]))
                return false;

            return true;
        }

        private bool IsImageExtension(string extension)
        {
            var isImage = false;
            switch (extension)
            {
                case ".gif":
                case ".jpg":
                case ".png":
                    isImage = true;
                    break;
                default:
                    isImage = false;
                    break;
            }
            return isImage;
        }
    }
}
