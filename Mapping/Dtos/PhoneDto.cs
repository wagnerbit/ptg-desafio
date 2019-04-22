using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

namespace WebApi.Mapping {
    public class PhoneDto {
        public int Number { get; set; }
        public int Area_code { get; set; }
        public string Country_code { get; set; }
        public virtual int UsersFK { get; set; }
        public virtual User User { get; set; }

    }
}