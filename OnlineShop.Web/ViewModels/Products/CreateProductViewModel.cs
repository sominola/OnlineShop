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
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int Price { get; set; }
        
        [Required]
        public SizeProduct Size { get; set; }
        
        [Required]
        public ColorProduct Color { get; set; }
        
        [Required]
        public string Description { get; set; }

        public IFormFileCollection Files { get; set; }
    }
}