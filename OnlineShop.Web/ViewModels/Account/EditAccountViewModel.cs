using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OnlineShop.Web.ViewModels.Account
{
    public class EditAccountViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string AvatarPath { get; set; }

        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public IFormFile File { get; set; }

        [Required]
        public string Id { get; init; }
    }
}