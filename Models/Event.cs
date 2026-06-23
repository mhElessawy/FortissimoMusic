using System.ComponentModel.DataAnnotations;

namespace Fortissimo.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الفعالية مطلوب")]
        [Display(Name = "العنوان")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "التاريخ مطلوب")]
        [Display(Name = "التاريخ")]
        [DataType(DataType.DateTime)]
        public DateTime EventDate { get; set; }

        [Display(Name = "المكان")]
        public string? Location { get; set; }

        [Display(Name = "صورة الفعالية")]
        public string? ImageUrl { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "تاريخ الإضافة")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
