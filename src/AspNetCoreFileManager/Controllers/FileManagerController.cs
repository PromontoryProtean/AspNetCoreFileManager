//TODO: add some kind of security to download action and find a replacement for System.Net.Mime.ContentDisposition in .NET Core
//TODO: find a way to store UiNotificationMessage list in TempData and implements the UiNotificationMessageBus

using AspNetCoreFileManager.FileManager;
using AspNetCoreFileManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace AspNetCoreFileManager.Web.Controllers
{
    [Route("file-manager")]
    public class FileManagerController : Controller
    {
        private readonly IFileManagerService _fileManagerService;

        public FileManagerController(IFileManagerService fileManagerService)
        {
            _fileManagerService = fileManagerService;
        }

        public IActionResult Index(string dir)
        {
            dir = string.IsNullOrWhiteSpace(dir) ? "/media" : dir;

            var fileManager = new FileManagerViewModel();
            fileManager.Breadcrumbs = _fileManagerService.GetBreadcrumbs(dir);
            fileManager.FileManagerItems = _fileManagerService.GetFileManagerItems(dir);
            fileManager.CurrentDirectory = dir;

            var fileManagerViewModeCookie = Request.Cookies["FileManagerViewMode"];
            if (string.IsNullOrWhiteSpace(fileManagerViewModeCookie))
                fileManager.ViewMode = FileManagerViewMode.Details;
            else if (fileManagerViewModeCookie == "Details")
                fileManager.ViewMode = FileManagerViewMode.Details;
            else
                fileManager.ViewMode = FileManagerViewMode.Tiles;

            return View(fileManager);
        }

        [Route("create-directory")]
        [HttpPost]
        public IActionResult CreateDirectory(string dir, FileManagerDirectoryViewModel directory)
        {
            if (!ModelState.IsValid)
                return View(directory);

            dir = string.IsNullOrWhiteSpace(dir) ? "/media" : dir;
            var newDirectoryPath = dir + "/" + directory.Name.Slugify();
            _fileManagerService.CreateDirectory(newDirectoryPath);

            TempData["MessageBusSuccess"] = "The Directory was created successfully.";
            //_uiNotificationMessageBus.AddSuccessMessage("The directory was created successfully.");
            return RedirectToAction("Index", new { dir = newDirectoryPath });
        }

        [Route("delete-directory")]
        public IActionResult DeleteDirectory(string dir)
        {
            _fileManagerService.DeleteDirectory(dir);

            TempData["MessageBusSuccess"] = "The directory was deleted successfully.";
            //_uiNotificationMessageBus.AddSuccessMessage("The directory was deleted successfully.");
            return RedirectToAction("Index", new { dir = dir.Substring(0, dir.LastIndexOf("/")) });
        }

        [Route("delete-file")]
        public IActionResult DeleteFile(string file)
        {
            _fileManagerService.DeleteFile(file);

            TempData["MessageBusSuccess"] = "The file was deleted successfully.";
            //_uiNotificationMessageBus.AddSuccessMessage("The file was deleted successfully.");
            return RedirectToAction("Index", new { dir = file.Substring(0, file.LastIndexOf("/")) });
        }

        [Route("bulk-delete")]
        [HttpPost]
        public IActionResult BulkDelete(string[] deletePaths)
        {
            if (deletePaths == null || deletePaths.Length == 0)
            {
                TempData["MessageBusAlert"] = "Bulk delete failed. Directories and/or files must be selected.";
                //_uiNotificationMessageBus.AddAlertMessage("Bulk delete failed. Directories and/or files must be selected.");
                return RedirectToAction("Index");
            }

            _fileManagerService.BulkDelete(deletePaths);

            TempData["MessageBusSuccess"] = "The files were deleted successfully.";
            //_uiNotificationMessageBus.AddSuccessMessage("The files were deleted successfully.");
            var returnDir = deletePaths[0].Substring(0, deletePaths[0].LastIndexOf("/"));
            return RedirectToAction("Index", new { dir = returnDir });
        }

        [Route("download-file")]
        public FileResult DownloadFile(string file)
        {
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + Path.GetFileName(file));
            //return File(file, "application/octet-stream");
            return PhysicalFile(_fileManagerService.GetFileDownloadPath(file), "application/octet-stream");
        }

        [Route("upload-file")]
        [HttpPost]
        public ActionResult UploadFile(string dir, string name, int? chunks, int? chunk, IFormFile file)
        {
            _fileManagerService.UploadFile(dir, name, chunks, chunk, file);
            return Content("chunk uploaded", "text/plain");
        }
    }
}