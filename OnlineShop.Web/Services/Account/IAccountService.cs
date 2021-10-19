using OnlineShop.Data.Models;
using OnlineShop.Web.ViewModels.Account;

namespace OnlineShop.Web.Services.Account
{
    public interface IAccountService
    {
        EditAccountViewModel GetAccountViewModel(User user);
        void UpdateUserData(User user, EditAccountViewModel model);
        User CreateNewUser(RegisterViewModel model);
    }
}