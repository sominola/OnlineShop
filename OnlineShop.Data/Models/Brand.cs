using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Data.Models
{
    public class Brand
    {
        [Required]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}