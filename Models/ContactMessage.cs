using System.ComponentModel.DataAnnotations;

namespace Fortissimo.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [Display(Name = "الاسم")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "بريد إلكتروني غير صحيح")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "رقم الهاتف")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "الرسالة مطلوبة")]
        [Display(Name = "الرسالة")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "تاريخ الإرسال")]
        public DateTime SentAt { get; set; } = DateTime.Now;

        [Display(Name = "تمت القراءة")]
        public bool IsRead { get; set; } = false;
    }
}
