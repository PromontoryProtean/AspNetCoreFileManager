//TODO: is WebUtility.UrlDecode the ideal class to call from? WebUtility vs System.Text.Encodings.Web.HtmlEncoder vs IHtmlEncoder
//TODO: read from config to set UserUploadDirecttory
//TODO: implement some kind of image filtering
//TODO: find a replacement for System.Web.HttpException
//TODO: verify write permission before creating directory or uploading a file
//TODO: complete UploadFile

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace AspNetCoreFileManager.FileManager
{
    public class FileManagerService : IFileManagerService
    {
        private readonly HttpContext _httpContext;

        private readonly IHostingEnvironment _hostingEnvironment;

        public FileManagerService(IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _hostingEnvironment = hostingEnvironment;
        }

        private string GetFileSystemPath(string currentDirectory)
        {
            if (!string.IsNullOrWhiteSpace(currentDirectory) && currentDirectory.StartsWith("/secure-files"))
            {
                return _hostingEnvironment.ContentRootPath + "\\Uploads" + currentDirectory.Replace("/secure-files", "").Replace(" / ", "\\");
            }
            else
            {
                currentDirectory = string.IsNullOrWhiteSpace(currentDirectory) ? "/media" : currentDirectory;
                if (!currentDirectory.StartsWith("/media") || (currentDirectory.Length > 6 && !currentDirectory.StartsWith("/media/")))
                {
                    //throw new System.Web.HttpException("Access is denied on the upload directory.");
                    throw new Exception("Access is denied on the upload directory.");
                }

                return _hostingEnvironment.WebRootPath + currentDirectory.Replace("/", "\\");
            }
        }

        public string GetBreadcrumbs(string currentDirectory)
        {
            if (string.IsNullOrWhiteSpace(currentDirectory))
                currentDirectory = "/media";

            var breadcrumbs = new StringBuilder();
            var url = new StringBuilder();
            var dirCount = currentDirectory.Length - currentDirectory.Replace("/", "").Length;

            var i = 0;
            foreach (var directoryName in currentDirectory.Remove(0, 1).Split('/'))
            {
                url.Append("/" + directoryName);
                i += 1;
                if (i == dirCount)
                    breadcrumbs.Append("<li>" + directoryName + "</li>");
                else
                    breadcrumbs.Append("<li><a href=\"" + _httpContext.Request.Path.ToString() + "?dir=" + WebUtility.UrlEncode(url.ToString()) + "\">" + directoryName + "</a></li>");
            }

            breadcrumbs.Insert(0, "<ol class=\"breadcrumb\">");
            breadcrumbs.Append("</ol>");

            return breadcrumbs.ToString();
        }

        public IEnumerable<FileManagerItem> GetFileManagerItems(string currentDirectory)
        {
            if (string.IsNullOrWhiteSpace(currentDirectory))
                currentDirectory = "/media";

            var directoryInfo = new DirectoryInfo(GetFileSystemPath(currentDirectory));
            var files = directoryInfo.GetFileSystemInfos();

            var fmFolder = new List<FileManagerItem>();
            var fmFiles = new List<FileManagerItem>();

            foreach (var fileSystemInfo in files)
            {
                var fmi = new FileManagerItem();
                fmi.Name = fileSystemInfo.Name;
                fmi.LastModified = fileSystemInfo.LastWriteTime;

                var requestPath = _httpContext.Request.Path.ToString();
                if (fileSystemInfo is DirectoryInfo)
                {
                    fmi.ItemType = FileManagerItemType.Directory;
                    fmi.BrowseUrl = requestPath + "?dir=" + WebUtility.UrlEncode(currentDirectory + "/" + fileSystemInfo.Name); //QueryHelpers.AddQueryString(urlLocalPath, qs);
                    fmi.DeleteUrl = requestPath + "/delete-directory?dir=" + WebUtility.UrlEncode(currentDirectory + "/" + fileSystemInfo.Name);
                    fmi.ThumbnailUrl = "/images/icons/folder.png";
                    fmFolder.Add(fmi);
                }
                else
                {
                    //if (!string.IsNullOrWhiteSpace(fileType) && fileType.ToLower() == "image")
                    //{
                    //    if (ImageResizer.Configuration.Config.Current.Pipeline.IsAcceptedImageType(f.Name))
                    //        l.Add(f);
                    //}
                    //else
                    fmi.ItemType = FileManagerItemType.File;
                    fmi.Size = (((FileInfo)fileSystemInfo).Length / 1000).ToString() + " KB";
                    fmi.BrowseUrl = requestPath;
                    fmi.DeleteUrl = requestPath + "/delete-file?file=" + WebUtility.UrlEncode(currentDirectory + "/" + fileSystemInfo.Name);
                    fmi.ThumbnailUrl = currentDirectory + "/" + fileSystemInfo.Name + "?width=125&height=125";
                    fmFiles.Add(fmi);
                }
            }

            var fileManagerItems = new List<FileManagerItem>();
            fileManagerItems.AddRange(fmFolder);
            fileManagerItems.AddRange(fmFiles);

            return fileManagerItems;
        }

        public void CreateDirectory(string directoryPath)
        {
            Directory.CreateDirectory(GetFileSystemPath(directoryPath));
        }

        public void DeleteDirectory(string directoryPath)
        {
            Directory.Delete(GetFileSystemPath(directoryPath), true);
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(GetFileSystemPath(filePath));
        }

        public void BulkDelete(IEnumerable<string> deletePaths)
        {
            foreach (string deletePath in deletePaths)
            {
                var physicalPath = GetFileSystemPath(deletePath);
                if ((File.GetAttributes(physicalPath) & FileAttributes.Directory) == FileAttributes.Directory)
                    Directory.Delete(physicalPath, true);
                else
                    File.Delete(physicalPath);
            }
        }

        public string GetFileDownloadPath(string directoryPath)
        {
            return GetFileSystemPath(directoryPath);
        }

        public void UploadFile(string dir, string name, int? chunks, int? chunk, IFormFile formFile)
        {
            //var l = new LogWriter("Logs/Plupload.txt");
            //l.WriteLine("Starting upload.");

            //bool hasPermission = Utilities.VerifyWritePermission(uploadDirectory);
            //if (!hasPermission)
            //{
            //    context.Response.StatusCode = 500;
            //    context.Response.Write("{\"jsonrpc\" : \"2.0\", \"error\" : {\"code\": 400, \"message\": \"Permissions have not been set correctly on the currently selected folder.\"}, \"id\" : \"id\"}");
            //    return;
            //}

            //string fileName = string.IsNullOrWhiteSpace(name)?  string.Empty;
            //fileName = Path.GetFileNameWithoutExtension(fileName).Slugify() + Path.GetExtension(fileName).ToLower();

            chunks = chunks ?? 0;
            chunk = chunk ?? 0;
            bool isLastChunk = (chunk >= (chunks - 1)) ? true : false;
            //l.WriteLine("Chunk " + (chunk + 1).ToString() + " of " + chunks.ToString() + ". IsLastChunk: " + isLastChunk.ToString());

            var fileName = Path.GetFileNameWithoutExtension(name).Slugify() + Path.GetExtension(name).ToLower();
            using (var fs = new FileStream(GetFileSystemPath(dir) + "\\" + fileName, chunk == 0 ? FileMode.Create : FileMode.Append))
            using (var reader = new BinaryReader(formFile.OpenReadStream()))
            {
                fs.Write(reader.ReadBytes(Convert.ToInt32(formFile.Length)), 0, Convert.ToInt32(formFile.Length));
            }

            //if (isLastChunk && ImageResizer.Configuration.Config.Current.Pipeline.IsAcceptedImageType(fileName))
            //{
            //    // check the querystring for image properties and validate them if they exist
            //    string widthQs = context.Request.QueryString["w"];
            //    string heightQs = context.Request.QueryString["h"];
            //    int width = 0;
            //    if (widthQs.IsNumeric())
            //        width = Convert.ToInt32(widthQs);
            //    if (width > 4000)
            //        width = 0;
            //    int height = 0;
            //    if (heightQs.IsNumeric())
            //        height = Convert.ToInt32(heightQs);
            //    if (height > 4000)
            //        height = 0;

            //    //l.WriteLine("Dimensions: " + width + "x" + height);

            //    if ((width > 0 || height > 0))
            //    {
            //        var sourceFilePath = Path.Combine(savePath, fileName);
            //        var newFilePath = Path.Combine(savePath, Guid.NewGuid().ToString());

            //        ImageResizer.ImageJob i = new ImageResizer.ImageJob();
            //        i.Source = sourceFilePath;
            //        i.Dest = newFilePath;
            //        i.Instructions = new ImageResizer.Instructions();
            //        i.Instructions.Mode = ImageResizer.FitMode.Max;
            //        if (width > 0)
            //            i.Instructions.Width = width;
            //        if (height > 0)
            //            i.Instructions.Height = height;
            //        i.Build();
            //        File.Delete(sourceFilePath);
            //        File.Move(newFilePath, sourceFilePath);
            //    }
            //}
        }
    }
}