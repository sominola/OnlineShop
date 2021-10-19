using OnlineShop.Data.Models;
using OnlineShop.Web.ViewModels.Account;

namespace OnlineShop.Web.Services.Account
{
    public class AccountService : IAccountService
    {
        public EditAccountViewModel GetAccountViewModel(User user)
        {
            return new EditAccountViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                Login = user.Login,
                Name = user.Name,
                LastName = user.LastName,
            };
        }

        public void UpdateUserData(User user, EditAccountViewModel model)
        {
            user.Email = model.Email;
            user.Login = model.Login;
            user.Name = model.Name;
            user.LastName = model.LastName;
        }

        public User CreateNewUser(RegisterViewModel model)
        {
            return new User()
            {
                Name = model.Name,
                LastName = model.LastName,
                Login = model.Login, 
                Email = model.Email,
            };
        }
    }
}