using AspNetCoreFileManager.FileManager;
using System.Collections.Generic;

namespace AspNetCoreFileManager.Models
{
    public class FileManagerViewModel
    {
        public string CurrentDirectory { get; set; }

        public string Breadcrumbs { get; set; }

        public IEnumerable<FileManagerItem> FileManagerItems { get; set; }

        public FileManagerViewMode ViewMode { get; set; }
    }
}