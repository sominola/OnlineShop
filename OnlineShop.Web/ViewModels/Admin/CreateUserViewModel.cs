using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Web.ViewModels.Admin
{
    public class CreateUserViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Field {0} must have a minimum of {2} and a maximum of {1} characters.", MinimumLength = 5)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Id { get; init; }
    }
}