﻿using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Web.Models.ViewModels
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
    }
}