using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

namespace WebApi.Dtos {
    public class UserDto {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Phone> Phones { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public virtual DateTime Last_Login { get; set; }
        public virtual DateTime Created_At { get; set; }
        public string Password { get; set; }
    }
}