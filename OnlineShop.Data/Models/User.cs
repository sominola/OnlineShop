using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace OnlineShop.Data.Models
{
    public class User: IdentityUser
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int? Year { get; set; }
    }
}