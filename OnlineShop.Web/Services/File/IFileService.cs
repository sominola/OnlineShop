using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OnlineShop.Web.Services.File
{
    public interface IFileService
    {
        Task WriteFileAsync(string path, IFormFile file);
        Task WriteFilesAsync(IEnumerable<string> path, IFormFileCollection files);
    }
}