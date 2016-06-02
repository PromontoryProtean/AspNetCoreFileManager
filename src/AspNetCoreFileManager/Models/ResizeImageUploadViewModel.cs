using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreFileManager.Models
{
    public class ResizeImageUploadViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public int Width { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public int Height { get; set; }

        [Required]
        public IFormFile FormFile { get; set; }

        public ResizeImageUploadViewModel()
        {
            Width = 250;
            Height = 250;
        }
    }
}