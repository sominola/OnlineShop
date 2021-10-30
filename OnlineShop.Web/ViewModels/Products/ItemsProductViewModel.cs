using System.Collections.Generic;
using OnlineShop.Data.Models;

namespace OnlineShop.Web.ViewModels.Products
{
    public class ItemsProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }
    }
}