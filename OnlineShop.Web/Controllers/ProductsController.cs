using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data.Models;
using OnlineShop.Web.Services.File;
using OnlineShop.Web.Services.Products;
using OnlineShop.Web.ViewModels.Products;

namespace OnlineShop.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IImageService _imageService;

        public ProductsController(IProductService productService, IImageService imageService)
        {
            _productService = productService;
            _imageService = imageService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Item(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product != null)
            {
                return View(product);
            }

            ModelState.AddModelError(string.Empty, "Product don't found");

            return View();
        }

        public IActionResult Create()
        {
            var model = new CreateProductViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            var images = await _imageService.UploadImagesAsync(model.Files);
            var product = new Product
            {
                Description = model.Description,
                Name = model.Name,
                Price = model.Price,
                ColorProduct = model.Color,
                SizeProduct = model.Size,
                
                Images = images as List<ProductImage>
            };

            await _productService.CreateProductAsync(product);

            return View();
        }
    }
}