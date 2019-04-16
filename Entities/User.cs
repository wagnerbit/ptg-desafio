namespace WebApi.Entities {
    public class User {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        //public Phone[] Phones { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }

    // public class Phone {
    //     public int Number { get; set; }
    //     public int AreaCode { get; set; }
    //     public string CountryCode { get; set; }
    // }
}