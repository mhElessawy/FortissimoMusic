using System.ComponentModel.DataAnnotations;

namespace Fortissimo.Models
{
    public class SliderImage
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "عنوان الصورة")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "مسار الصورة")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "الترتيب")]
        public int Order { get; set; } = 0;

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;
    }
}
