namespace ProniaWebApp.ViewModels.Account
{
    public class RegisterVM
    {

        [Required]
        [MinLength(3)]
        [MaxLength(25)]
        public string Name { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(25)]
        public string Surname { get; set; }
        [Required]
        [MaxLength(25)]
        public string Username { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare(nameof(Password))]
        [MinLength(8)]
        public string ConfirmPassword { get; set; }

    }
}
