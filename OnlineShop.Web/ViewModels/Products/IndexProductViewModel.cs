using System.Collections.Generic;
using System.Linq;
using OnlineShop.Data.Models;

namespace OnlineShop.Web.ViewModels.Products
{
    public class IndexProductViewModel
    {
        public FilterViewModel Filter { get; set; }
        public int MaxCountOnPage { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public int TotalProducts { get; set; }
        public int CountProductsOnPage { get; set; }
        public int CountPages { get; set; }

        public IndexProductViewModel()
        {
            
        }

        public IndexProductViewModel(FilterViewModel filter)
        {
            Filter = filter;
        }

    }
}