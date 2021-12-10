using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OnlineShop.Data.Models;

namespace OnlineShop.Services.File
{
    public interface IImageService
    {
        Task<IEnumerable<SiteImage>> UploadImagesFromWebUrlAsync(List<string> urls);
        Task<IEnumerable<SiteImage>> UploadImagesFromWebAsync(List<string> urls);
        Task<SiteImage> UploadImageAsync(IFormFile file);

        Task<SiteImage> UploadImageAsync(IFormFile file, Guid id);
        Task<IEnumerable<SiteImage>> UploadImagesAsync(IFormFileCollection files);
    }
}