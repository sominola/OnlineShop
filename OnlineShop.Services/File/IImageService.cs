using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OnlineShop.Data.Models;

namespace OnlineShop.Services.File
{
    public interface IImageService
    {
        Task<IEnumerable<ProductImage>> UploadImagesFromWebUrlAsync(List<string> urls);
        Task<IEnumerable<ProductImage>> UploadImagesFromWebAsync(List<string> urls);
        Task<ProductImage> UploadImageAsync(IFormFile file);
        Task<IEnumerable<ProductImage>> UploadImagesAsync(IFormFileCollection files);
    }
}