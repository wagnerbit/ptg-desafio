using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos {
    public class PhoneDto {
        public int Number { get; set; }
        public int Area_code { get; set; }
        public string Country_code { get; set; }

    }
}