namespace ATON_Internship.Models
{
    public class DTOs
    {
        public class LoginRequest
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        public class CreateUserRequest
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public int Gender { get; set; }
            public DateTime? Birthday { get; set; }
            public bool IsAdmin { get; set; }
        }

        public class UpdateUserDetailsRequest
        {
            public string NewName { get; set; }
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
