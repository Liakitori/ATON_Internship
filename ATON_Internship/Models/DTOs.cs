using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ATON_Internship.Models
{
    public class DTOs
    {
        public class LoginRequest
        {
            [Required]
            [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Логин может содержать только латинские буквы и цифры.")]
            public string Login { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Пароль может содержать только латинские буквы и цифры.")]
            public string Password { get; set; }
        }

        public class CreateUserRequest
        {
            [Required]
            [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Логин может содержать только латинские буквы и цифры.")]
            public string Login { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Пароль может содержать только латинские буквы и цифры.")]
            public string Password { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ]+$", ErrorMessage = "Имя может содержать только буквы русского/латинского алфавита.")]
            public string Name { get; set; }

            [Required]
            [Range(0, 2, ErrorMessage = "Пол может иметь только следующие значения: 0 (женщина), 1 (мужчина) или 2 (неизвестно).")]
            public int Gender { get; set; }

            public DateTime? Birthday { get; set; }
            public bool IsAdmin { get; set; }
        }

        public class UpdateUserDetailsRequest
        {
            [Required]
            [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ]+$", ErrorMessage = "Имя может содержать только буквы русского/латинского алфавита.")]
            public string NewName { get; set; }

            [Required]
            [Range(0, 2, ErrorMessage = "Пол может иметь только следующие значения: 0 (женщина), 1 (мужчина) или 2 (неизвестно).")]
            public int NewGender { get; set; }
            public DateTime? NewBirthday { get; set; }
        }

        public class UserDetailsDto
        {
            public string Name { get; set; }
            public int Gender { get; set; }
            public DateTime? Birthday { get; set; }
            public bool IsActive { get; set; }
        }

        public class UserDto
        {
            public string Login { get; set; }
            public string Name { get; set; }
            public int Gender { get; set; }
            public DateTime? Birthday { get; set; }
            public bool IsActive { get; set; }
        }



    }
}
