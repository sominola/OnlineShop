using System.ComponentModel.DataAnnotations;
using OnlineShop.Web.Extension.Validation;

namespace OnlineShop.Web.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailValidation(ErrorMessage = "Wrong Email")]
        public string Email { get; set; }
        
        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required] 
        [Display(Name = "Name")] 
        public string Name { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Field {0} must have a minimum of {2} and a maximum of {1} characters.",
            MinimumLength = 5)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password mismatch")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string PasswordConfirm { get; set; }
    }
}