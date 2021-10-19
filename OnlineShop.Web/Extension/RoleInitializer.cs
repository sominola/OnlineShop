using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OnlineShop.Data.Models;

namespace OnlineShop.Web.Extension
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            const string login = "admin";
            const string email = "admin@gmail.com";
            const string password = "admin";

            const string role = "Admin";

            if (await roleManager.FindByNameAsync(role) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            if (await userManager.FindByNameAsync(login) == null)
            {
                var admin = new User {Email = email, Login = login};
                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, role);
                }
            }
        }
    }
}