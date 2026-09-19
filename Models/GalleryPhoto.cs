using System.ComponentModel.DataAnnotations;

namespace Fortissimo.Models
{
    public class GalleryPhoto
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "اسم الملف")]
        public string FileName { get; set; } = string.Empty;

        [Display(Name = "العنوان")]
        public string? Title { get; set; }

        [Display(Name = "الترتيب")]
        public int Order { get; set; } = 0;

        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }
}
