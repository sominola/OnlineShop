using System;
using Microsoft.Extensions.DependencyInjection;
using OnlineShop.Services;
using OnlineShop.Services.BrandService;
using OnlineShop.Services.File;
using OnlineShop.Services.Parser;
using OnlineShop.Services.Products;

namespace OnlineShop.Web.Extension
{
    public static class ServicesExtension
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = Environment.GetEnvironmentVariable("GoogleId", EnvironmentVariableTarget.User)!;
                    options.ClientSecret =
                        Environment.GetEnvironmentVariable("GoogleSecret", EnvironmentVariableTarget.User)!;
                });

            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = Environment.GetEnvironmentVariable("FacebookId", EnvironmentVariableTarget.User)!;
                options.AppSecret =
                    Environment.GetEnvironmentVariable("FacebookSecret", EnvironmentVariableTarget.User)!;
            });


            services.AddScoped<IProductService, ProductsService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IBrandService, BrandService>();

            var configForParser = new ConfigProductParser
            {
                BaseAddress = new Uri("https://answear.ua"),
                PathToProducts = "k/vin/odyag/kofty",

                SelectorForProducts = "div.ProductItem__productCardImageWrapper__2eewd a",
                SelectorForPrice = "p.Price__currentPrice__22uD3",
                SelectorForBrandName = "figure.ProductCard__productNameAndLogoImage__1P5if img",
                SelectorForName = "div.ProductCard__productNameAndLogo__1NudT h1",
                SelectorForDescription = "p.ProductActive__descriptionParagraph__2DAC6",
                SelectorForSize = "div.ModelSizeInfo__sizeModelInfoWrapper__3WWC6",
                SelectorForImages =
                    "div.ProductCard__slide__27Is- div.Image__mediaLazyload__3TlcL picture.Image__cardImage__3eRwk  img"
            };
            Config.ConfigProductParser = configForParser;
            services.AddScoped<ProductParserService>();
        }
    }
}