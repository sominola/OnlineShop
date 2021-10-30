using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using OnlineShop.Data.Enums;

namespace OnlineShop.Web.ViewModels.Products
{
    public class CreateProductViewModel
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public SizeProduct Size { get; set; }
        
        [Required]
        public ColorProduct Color { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public IFormFileCollection Files { get; set; }
    }
}