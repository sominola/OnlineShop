using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace OnlineShop.Data.Models
{
    public class User : IdentityUser
    {
        public string Login
        {
            get => UserName;
            set => UserName = value;
        }

        public string Name { get; set; }
        public string LastName { get; set; }

        [NotMapped] 
        public string FullName => $"{Name}  {LastName}";
    }
}