using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace OnlineShop.Web.Services.File
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task WriteFileAsync(string path, IFormFile file)
        {
            try
            {
                await using var fileStream = new FileStream(_webHostEnvironment.WebRootPath + path, FileMode.Create);
                await file.CopyToAsync(fileStream);
            }
            catch (DirectoryNotFoundException e)
            {
                var directoryPath = _webHostEnvironment.WebRootPath + Path.GetDirectoryName(path);
                if(!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
                
                await using var fileStream = new FileStream(_webHostEnvironment.WebRootPath + path, FileMode.Create);
                await file.CopyToAsync(fileStream);
            }
        }

        public async Task WriteFilesAsync(IEnumerable<string> path, IFormFileCollection files)
        {
            var pathAndFiles = path.Zip(files, (p, f) => new {Path = p, File = f});

            var directoryPath = _webHostEnvironment.WebRootPath + Path.GetDirectoryName(path.FirstOrDefault());

            if(!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            foreach (var item in pathAndFiles)
            {
                await using var fileStream =
                    new FileStream(_webHostEnvironment.WebRootPath + item.Path, FileMode.Create);
                await item.File.CopyToAsync(fileStream);
            }
        }
        
    }
}