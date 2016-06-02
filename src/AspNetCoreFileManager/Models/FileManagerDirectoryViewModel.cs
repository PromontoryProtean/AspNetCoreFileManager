using System.ComponentModel.DataAnnotations;

namespace AspNetCoreFileManager.Models
{
    public class FileManagerDirectoryViewModel
    {
        [Display(Name = "Name")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}