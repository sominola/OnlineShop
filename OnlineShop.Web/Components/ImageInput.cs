using Microsoft.AspNetCore.Mvc;

namespace OnlineShop.Web.Components
{
    public class ImageInput : ViewComponent
    {
        public IViewComponentResult Invoke(bool isCreated = true)
        {
            return isCreated ? View() : View("InitImageInput");
        }
    }
}