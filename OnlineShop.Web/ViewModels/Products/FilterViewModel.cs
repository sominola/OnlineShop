using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShop.Data.Models;

namespace OnlineShop.Web.ViewModels.Products
{
    public class FilterViewModel
    {
        [BindProperty(Name="Page")]
        public int CurrentPage { get; set; } = 1;
        
        [BindProperty(Name="Name")]
        public string CurrentName { get; set; }
        
        [BindProperty(Name="Sort")]
        public OrderBy CurrentSort { get; set; } = OrderBy.PriceDesc;
        
        public SelectList Brands { get; set; }
        
        [BindProperty(Name="Brand")]
        public string CurrentBrand { get; set; }
        
        public string CurrentSortString => CurrentSort.ToString().ToLower();

        [BindProperty(Name="Count")]
        public int CountProductsOnPage { get; set; } = 15;
        public IEnumerable<SelectListItem> SelectListItems { get; set; }

        public FilterViewModel()
        {
            GenerateSortOrder();
        }
        

        private void GenerateSortOrder()
        {
            SelectListItems = new List<SelectListItem>
            {
                new("Sort low to high date", OrderBy.DateAsc.ToString().ToLower(), CurrentSort == OrderBy.DateAsc),
                new("Sort high to low date", OrderBy.DateDesc.ToString().ToLower(), CurrentSort == OrderBy.DateDesc),
                new("Sort low to high price", OrderBy.PriceAsc.ToString().ToLower(), CurrentSort == OrderBy.PriceAsc),
                new("Sort high to low price", OrderBy.PriceDesc.ToString().ToLower(), CurrentSort == OrderBy.PriceDesc),
                new("Sort low to high name", OrderBy.NameAsc.ToString().ToLower(), CurrentSort == OrderBy.NameAsc),
                new("Sort high to low name", OrderBy.NameDesc.ToString().ToLower(), CurrentSort == OrderBy.NameDesc),
            };
        }
    }
}