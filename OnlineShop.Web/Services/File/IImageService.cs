using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OnlineShop.Data.Models;

namespace OnlineShop.Web.Services.File
{
    public interface IImageService
    {
        Task<ProductImage> UploadImageAsync(IFormFile file);
        Task<IEnumerable<ProductImage>> UploadImagesAsync(IFormFileCollection files);
    }
}