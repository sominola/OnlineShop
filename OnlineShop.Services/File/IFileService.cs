using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OnlineShop.Services.File
{
    public interface IFileService
    {
        Task WriteFilesFromStreamAsync(string path, HttpContent content);
        Task WriteFileAsync(string path, IFormFile file);
        Task WriteFilesAsync(IEnumerable<string> path, IFormFileCollection files);
    }
}