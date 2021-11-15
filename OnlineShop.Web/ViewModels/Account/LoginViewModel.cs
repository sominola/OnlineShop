using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace OnlineShop.Web.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }
         
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
         
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
         
        public string ReturnUrl { get; set; }
        
        public IEnumerable<AuthenticationScheme> ExternalLogins { get; set; }
    }
}