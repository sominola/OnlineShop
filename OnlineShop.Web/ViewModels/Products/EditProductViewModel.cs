using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using OnlineShop.Data.Enums;
using OnlineShop.Data.Models;

namespace OnlineShop.Web.ViewModels.Products
{
    public class EditProductViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int Price { get; set; }
        
        [Required]
        public string BrandName { get; set; }
        
        [Required]
        public SizeProduct Size { get; set; }
        
        [Required]
        public ColorProduct Color { get; set; }
        
        [Required]
        public string Description { get; set; }

        public IList<EditProductImage> Images { get; set; }

        public IFormFileCollection Files { get; set; }
    }

    public class EditProductImage
    {
        
        public SiteImage Image { get; set; }
        public bool IsRemoved { get; set; }
    }
    
}