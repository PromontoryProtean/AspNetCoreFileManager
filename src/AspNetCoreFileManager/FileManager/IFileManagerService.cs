using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace AspNetCoreFileManager.FileManager
{
    public interface IFileManagerService
    {
        IEnumerable<FileManagerItem> GetFileManagerItems(string currentDirectory);

        string GetBreadcrumbs(string currentDirectory);

        void CreateDirectory(string directoryPath);

        void DeleteDirectory(string directoryPath);

        void DeleteFile(string filePath);

        void BulkDelete(IEnumerable<string> deletePaths);

        string GetFileDownloadPath(string directoryPath);

        void UploadFile(string dir, string name, int? chunks, int? chunk, IFormFile formFile);
    }
}
