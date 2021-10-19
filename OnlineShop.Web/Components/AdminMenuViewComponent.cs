using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShop.Web.Components
{
    public class AdminMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var menuItems = new List<AdminMenuItem>
            {
                new()
                {
                    DisplayValue = "User management",
                    ActionValue = "Index",
                    ControllerValue = "Admin"
                },
                new()
                {
                    DisplayValue = "Role management",
                    ActionValue = "Index",
                    ControllerValue = "Role"
                },
                new()
                {
                    DisplayValue = "Role of users",
                    ActionValue = "UserList",
                    ControllerValue = "Role"
                }
            };

            return View(menuItems);
        }
    }

    public class AdminMenuItem
    {
        public string DisplayValue { get; set; }
        public string ActionValue { get; set; }
        
        public string ControllerValue { get; set; }
    }
}