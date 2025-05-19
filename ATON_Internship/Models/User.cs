using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ATON_Internship.Models
{
    public class User
    {
        [Required]
        public required Guid Guid { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Логин может содержать только латинские буквы и цифры.")]
        public required string Login { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Пароль может содержать только латинские буквы и цифры.")]
        public required string Password { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ]+$", ErrorMessage = "Имя может содержать только буквы русского/латинского алфавита.")]
        public required string Name { get; set; }

        [Required]
        [Range(0,2, ErrorMessage = "Пол может иметь только следующие значения: 0 (женщина), 1 (мужчина) или 2 (неизвестно).")]
        public required int Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public bool Admin { get; set; }

        [Required]
        public required DateTime CreatedOn { get; set; }

        [Required]
        public required string CreatedBy { get; set; }

        [Required]
        public required DateTime ModifiedOn { get; set; }

        [Required]
        public required string ModifiedBy { get; set; }

        public DateTime? RevokedOn { get; set; }
        public string? RevokedBy { get; set; }
    }
}
