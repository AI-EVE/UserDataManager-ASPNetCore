using DataManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace DataManager.Core.AccountDTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Name is Required.")]
        public string PersonName { get; set; }

        [Required(ErrorMessage = "Email is Required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is mandatory")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "You must confirm the password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public Roles Role { get; set; } = Roles.User;
    }
}
