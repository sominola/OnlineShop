﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data.Models;
using OnlineShop.Services.BrandService;
using OnlineShop.Services.File;
using OnlineShop.Services.Parser;
using OnlineShop.Services.Products;
using OnlineShop.Web.ViewModels.Products;

// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace OnlineShop.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IImageService _imageService;
        private readonly ProductParserService _productParserService;
        private readonly IBrandService _brandService;

        public ProductsController(IProductService productService, IImageService imageService,
            ProductParserService productParserService, IBrandService brandService)
        {
            _productService = productService;
            _imageService = imageService;
            _productParserService = productParserService;
            _brandService = brandService;
        }

        public async Task<IActionResult> Index(FilterViewModel filter)
        {
            int skipProducts = (filter.CurrentPage - 1) * filter.CountProductsOnPage;
            
            var products = _productService.GetAllProducts();
        
            if (!await products.AnyAsync())
                return View(new IndexProductViewModel(filter));
            products = _productService.SortProductsByOrder(products, filter.CurrentSort);


            if (!string.IsNullOrEmpty(filter.CurrentName))
            {
                products = _productService.GetProductsByName(products, filter.CurrentName);
                if (!await products.AnyAsync())
                {
                    return View(new IndexProductViewModel(filter));
                }
            }
           
            
            if (!string.IsNullOrEmpty(filter.CurrentBrand) && filter.CurrentBrand != "All")
            {
                products = _productService.GetProductsByBrand(products, filter.CurrentBrand); 
                if (!await products.AnyAsync())
                {
                    return View(new IndexProductViewModel(filter));
                }
            }
            
            var brands = await _brandService.GetAllBrands().ToListAsync();
            brands.Insert(0, new Brand(){Name = "All", Id = Guid.Empty});
            filter.Brands = new SelectList(brands, "Name", "Name", filter.CurrentBrand);
            
            double totalProducts = await products.CountAsync();
            int maxPages = (int) Math.Ceiling(totalProducts / filter.CountProductsOnPage);

            //check if take > countProducts
            int takeProducts = filter.CountProductsOnPage;
            int skipTake = skipProducts + takeProducts;
            if (skipTake > totalProducts)
                takeProducts -= skipTake - (int) totalProducts;


            if (totalProducts > filter.CountProductsOnPage)
                products = _productService.SkipTakeProducts(products, skipProducts, takeProducts);

            if (filter.CurrentPage > maxPages || filter.CurrentPage <= 0)
                return RedirectToAction("Index", new {name = filter.CurrentName, sortOrder = filter.CurrentSort});

            var model = new IndexProductViewModel()
            {
                Filter = filter,
                Products = await products.ToListAsync(),
                CountPages = maxPages,
                TotalProducts = (int) totalProducts,
                MaxCountOnPage = filter.CountProductsOnPage,
                CountProductsOnPage = skipTake,
            };

            return View(model);
        }

        public async Task<IActionResult> Item(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product != null)
            {
                return View(product);
            }

            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new CreateProductViewModel();
            return View(model);
        }

        public async Task<IActionResult> Parser()
        {
            await _productParserService.StartParsing("k/vin/odyag/sorochky");
            await _productParserService.StartParsing("k/vin/odyag/kurtky-ta-palta");
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Files == null)
                {
                    ModelState.AddModelError(nameof(model.Files), "Files are empty");
                    return View();
                }

                var brand = await _brandService.FindBrandsByNameAsync(model.Name);
                if (brand == null)
                {
                    brand = new Brand()
                    {
                        Id = Guid.NewGuid(),
                        Name = model.BrandName
                    };
                   await _brandService.CreateBrand(brand);
                }
                var images = await _imageService.UploadImagesAsync(model.Files);
                var product = new Product
                {
                    Description = model.Description,
                    Name = model.Name,
                    Price = model.Price,
                    Brand = brand,
                    ColorProduct = model.Color,
                    DateCreated = DateTime.Now,
                    SizeProduct = model.Size,

                    Images = images as List<ProductImage>
                };

                await _productService.CreateProductAsync(product);
                return RedirectToAction("Index");
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var listImages = product.Images.Select(image => new EditProductImage {Image = image}).ToList();

            var model = new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                BrandName = product.Brand.Name,
                Price = product.Price,
                Size = product.SizeProduct,
                Color = product.ColorProduct,
                Description = product.Description,
                Images = listImages
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                List<ProductImage> images = new();
                if (model.Images != null)
                {
                    images.AddRange(from image in model.Images where !image.IsRemoved select image.Image);
                }

                if (model.Files != null)
                {
                    var imagesFromFile = await _imageService.UploadImagesAsync(model.Files);
                    images.AddRange(imagesFromFile);
                }
                var brand = await _brandService.FindBrandsByNameAsync(model.Name);
                if (brand == null)
                {
                    brand = new Brand()
                    {
                        Id = Guid.NewGuid(),
                        Name = model.BrandName
                    };
                    await _brandService.CreateBrand(brand);
                }

                var product = new Product
                {
                    Id = model.Id,
                    Name = model.Name,
                    Price = model.Price,
                    Brand = brand,
                    SizeProduct = model.Size,
                    ColorProduct = model.Color,
                    Description = model.Description,
                    Images = images
                };
                await _productService.UpdateProductAsync(product);
            }
            else
            {
                return View(model);
            }

            return RedirectToAction("Edit", model.Id);
        }
    }
}