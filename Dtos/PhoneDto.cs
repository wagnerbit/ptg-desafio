using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos {
    public class PhoneDto {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Area_Code { get; set; }
        public string Country_Code { get; set; }

    }
}