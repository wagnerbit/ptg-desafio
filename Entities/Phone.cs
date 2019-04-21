using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities {
    public class Phone {

        public int PhoneId { get; set; }
        public int Number { get; set; }
        public int Area_code { get; set; }
        public string Country_code { get; set; }
        public virtual int UsersFK { get; set; }
        public virtual User User { get; set; }
    }
}