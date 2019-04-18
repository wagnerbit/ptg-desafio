using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities {
    public class Phone {

        public int Number { get; set; }

        public int Area_code { get; set; }

        public string Country_code { get; set; }

        public virtual long UsersId { get; set; }

        public virtual User Users { get; set; }

    }
}