using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using OnlineShop.Data.Enums;
using OnlineShop.Data.Models;
using OnlineShop.Services.BrandService;
using OnlineShop.Services.File;
using OnlineShop.Services.Products;

namespace OnlineShop.Services.Parser
{
    public class ConfigProductParser
    {
        public Uri BaseAddress { get; init; }
        public string PathToProducts { get; init; }

        public string SelectorForProducts { get; init; }
        public string SelectorForPrice { get; init; }
        public string SelectorForName { get; init; }
        public string SelectorForDescription { get; init; }
        public string SelectorForSize { get; init; }

        public string SelectorForBrandName { get; set; }

        public string SelectorForImages { get; init; }
        // public string SelectorForColor { get; set; }
    }

    public class ProductParserService
    {
        private readonly HttpClient _client;
        private readonly IImageService _imageService;
        private readonly IProductService _productService;
        private readonly ConfigProductParser _config;
        private readonly ILogger<ProductParserService> _logger;
        private readonly IBrandService _brandService;

        public ProductParserService(IImageService imageService, IProductService productService,
            ILogger<ProductParserService> logger, IBrandService brandService)
        {
            _config = Config.ConfigProductParser ??
                      throw new NullReferenceException(nameof(Config.ConfigProductParser));

            _client = new HttpClient
            {
                BaseAddress = _config.BaseAddress
            };

            _imageService = imageService;
            _productService = productService;
            _logger = logger;
            _brandService = brandService;
        }

        public async Task StartParsing(string path = null)
        {
            var listPathProducts = await GetUrlsProducts(path ??= _config.PathToProducts);
            var pathProducts = listPathProducts as string[] ?? listPathProducts.ToArray();

            await foreach (var htmlProduct in GetHtmlProductList(pathProducts))
            {
                var pathImages = GetPathImagesProduct(htmlProduct) as List<string>;
                if (pathImages == null || !pathImages.Any())
                {
                    continue;
                }

                var price = GetPriceProduct(htmlProduct);
                var name = GetNameProduct(htmlProduct);
                var description = GetDescriptionProduct(htmlProduct);
                var size = GetSizeProduct(htmlProduct); 
                var color = GetColorProduct(htmlProduct);
                var brandName = GetBrandProduct(htmlProduct);

                var brand = await _brandService.FindBrandsByNameAsync(brandName);
                if (brand == null)
                {
                    brand = new Brand()
                    {
                        Id = Guid.NewGuid(),
                        Name = brandName,
                    };
                    await _brandService.CreateBrand(brand);
                }

                // var imagesProducts = await _imageService.UploadImagesFromWebAsync(pathImages);
                var imagesProducts = await _imageService.UploadImagesFromWebUrlAsync(pathImages);
                var product = new Product
                {
                    Description = description,
                    Name = name,
                    Price = price,
                    ColorProduct = color,
                    DateCreated = DateTime.Now,
                    SizeProduct = size,
                    Brand = brand,
                    Images = imagesProducts as List<ProductImage>
                };
                await _productService.CreateProductAsync(product);
                LogParseProduct(product);
            }
        }

        private ColorProduct GetColorProduct(IDocument htmlProduct)
        {
            var values = Enum.GetValues(typeof(ColorProduct));
            var random = new Random();

            return (ColorProduct) values.GetValue(random.Next(values.Length));
        }

        private string GetBrandProduct(IDocument document)
        {
            var brandElement = document.QuerySelector(_config.SelectorForBrandName);
            if (brandElement == null)
            {
                throw new NullReferenceException(nameof(brandElement));
            }

            return brandElement.GetAttribute("alt");
        }

        private void LogParseProduct(Product product)
        {
            _logger.LogInformation($"Id: {product.Id}");
            _logger.LogInformation($"Name: {product.Name}");
            _logger.LogInformation($"Price: {product.Price}");
            _logger.LogInformation($"Brand: {product.Brand}");
            _logger.LogInformation($"Color: {product.ColorProduct.ToString()}");
            _logger.LogInformation($"Size: {product.SizeProduct}");
            foreach (var image in product.Images)
            {
                _logger.LogInformation($"Image: {image.Id} {image.Name} {image.Path}");
            }
        }

        private async Task<IEnumerable<string>> GetUrlsProducts(string path)
        {
            var doc = await GetHtml(path);
            var test = doc.QuerySelectorAll(_config.SelectorForProducts);
            var links = new List<string>();
            foreach (var image in test)
            {
                var link = image.GetAttribute("href");
                if (link != null)
                {
                    links.Add(link);
                    _logger.LogInformation($"Link: {link}");
                }
            }

            return links;
        }

        private async IAsyncEnumerable<IDocument> GetHtmlProductList(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                var doc = await GetHtml(path);
                yield return doc;
            }
        }

        private int GetPriceProduct(IParentNode document)
        {
            var priceElement = document.QuerySelector(_config.SelectorForPrice);
            if (priceElement == null)
            {
                throw new NullReferenceException(nameof(priceElement));
            }

            var priceString = priceElement.Text().Replace(" грн", "");
            return int.Parse(priceString);
        }

        private string GetNameProduct(IParentNode document)
        {
            var name = document.QuerySelector(_config.SelectorForName);
            name ??= document.QuerySelector("div.ProductCard__productName__g0iPL");
            if (name == null)
            {
                throw new NullReferenceException(nameof(name));
            }

            return name.Text();
        }

        private string GetDescriptionProduct(IParentNode document)
        {
            var description = document.QuerySelector(_config.SelectorForDescription);
            if (description == null)
            {
                throw new NullReferenceException(nameof(description));
            }

            return description.Text();
        }

        private SizeProduct GetSizeProduct(IParentNode document)
        {
            var size = document.QuerySelector(_config.SelectorForSize);
            // if (size == null)
            // {
            //     throw new NullReferenceException(nameof(size));
            // }
            if (size == null)
            {
                var values = Enum.GetValues(typeof(SizeProduct));
                var random = new Random();

                var randomSize = (SizeProduct) values.GetValue(random.Next(values.Length));
                return randomSize;
            }

            return Enum.TryParse<SizeProduct>(size.Text().Last().ToString(), true, out var sizeProduct)
                ? sizeProduct
                : default;
        }

        private IEnumerable<string> GetPathImagesProduct(IParentNode document)
        {
            var listElements = document.QuerySelectorAll(_config.SelectorForImages);

            var listImages = new List<string>();

            foreach (var imageElement in listElements)
            {
                var image = imageElement.GetAttribute("src");
                if (image == null) continue;

                image = image.Replace("82x124", "1080x1626");
                listImages.Add(image);
            }

            return listImages;
        }

        private async Task<IDocument> GetHtml(string url)
        {
            var html = await _client.GetStreamAsync(url);
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var doc = await context.OpenAsync(req => req.Content(html));
            return doc;
        }
    }
}