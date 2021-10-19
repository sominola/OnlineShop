using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Web.ViewModels.Account
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string Login { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Field {0} must have a minimum of {2} and a maximum of {1} characters.", MinimumLength = 5)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
 
        [Required]
        [Compare("NewPassword", ErrorMessage = "Password mismatch")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password Confirm")]
        public string NewPasswordConfirm { get; set; }
    }
}