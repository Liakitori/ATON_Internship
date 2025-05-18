using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ATON_Internship.Models
{
    public class User
    {
        [Required]
        public Guid Guid { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Логин может содержать только латинские буквы и цифры.")]
        public string Login { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Пароль может содержать только латинские буквы и цифры.")]
        public string Password { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Имя может содержать только латинские буквы и цифры.")]
        public string Name { get; set; }

        [Range(0,2, ErrorMessage = "Пол может иметь следующие значения: 0/1/2, где 0 - женщина, 1 - мужчина или 2 - неизвестно.")]
        public int Gender { get; set; }

        public DateTime? Birthday { get; set; }
        public bool Admin { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? RevokedOn { get; set; }
        public string? RevokedBy { get; set; }
    }
}
