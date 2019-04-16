using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos {
    public class UserDto {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        // public Phone[] Phones { get; set; }

        public string Password { get; set; }
    }
}