using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OnlineShop.Data.Enums;

namespace OnlineShop.Data.Models
{
    public class Product
    {
        [Required]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        
        public SizeProduct SizeProduct { get; set; }
        
        public ColorProduct ColorProduct { get; set; }
        
        public string Description { get; set; }
        
        public DateTime DateCreated { get; set; }

        public List<ProductImage> Images { get; set; }
    }
}