using System.ComponentModel.DataAnnotations;

namespace Fortissimo.Models
{
    public class VideoClip
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "اسم الملف")]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "العنوان")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Display(Name = "الترتيب")]
        public int Order { get; set; } = 0;

        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }
}
