using System;
using System.IO;

namespace AspNetCoreFileManager.FileManager
{
    public class FileManagerItem
    {
        public string Name { get; set; }

        public DateTime LastModified { get; set; }

        public string Size { get; set; }

        private string _iconClass;
        public string IconClass
        {
            get
            {
                if (_iconClass != null)
                    return _iconClass;

                if (ItemType == FileManagerItemType.File)
                {
                    switch (Path.GetExtension(Name).ToLower())
                    {
                        case ".bmp":
                        case ".gif":
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".tif":
                            _iconClass = "fa fa-file-image-o fa-fw";
                            break;
                        case ".avi":
                        case ".fla":
                        case ".mpeg":
                        case ".mpg":
                        case ".swf":
                            _iconClass = "fa fa-file-video-o fa-fw";
                            break;
                        case ".aac":
                        case ".aif":
                        case ".au":
                        case ".flac":
                        case ".m4a":
                        case ".midi":
                        case ".mp3":
                        case ".ogg":
                        case ".wav":
                        case ".wma":
                            _iconClass = "fa fa-file-audio-o fa-fw";
                            break;
                        case ".doc":
                        case ".docx":
                            _iconClass = "fa fa-file-word-o fa-fw";
                            break;
                        case ".pdf":
                            _iconClass = "fa fa-file-pdf-o fa-fw";
                            break;
                        case ".ppt":
                        case ".pptx":
                            _iconClass = "fa fa-file-powerpoint-o fa-fw";
                            break;
                        case ".txt":
                            _iconClass = "fa fa-file-text-o fa-fw";
                            break;
                        case ".xls":
                        case ".xlsx":
                            _iconClass = "fa fa-file-excel-o fa-fw";
                            break;
                        case ".zip":
                            _iconClass = "fa fa-file-archive-o fa-fw";
                            break;
                        default:
                            _iconClass = "fa fa-file-o fa-fw";
                            break;
                    }
                }
                else
                {
                    _iconClass = "fa fa-folder-open fa-fw icon-yellow";
                }

                return _iconClass;
            }
        }

        public FileManagerItemType ItemType { get; set; }

        public string DownloadUrl { get; set; }

        public string BrowseUrl { get; set; }

        public string DeleteUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        private string _deleteWarning;
        public string DeleteWarning
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_deleteWarning))
                    _deleteWarning = ItemType == FileManagerItemType.Directory ? "Deleting a directory will delete all subdirectories and files. Are you sure you want to delete this directory?" : "Are you sure you want to delete this file?";
                return _deleteWarning;
            }
        }
    }
}