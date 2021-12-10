using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OnlineShop.Services.File
{
    public class FileService : IFileService
    {
        private readonly string _webRootPath;

        public FileService()
        {
            _webRootPath = Config.WebRootPath;
        }

        public async Task WriteFilesFromStreamAsync(string path, HttpContent content)
        {
            try
            {
                await using var fileStream = new FileStream(_webRootPath + path, FileMode.Create);
                await content.CopyToAsync(fileStream);
            }
            catch (DirectoryNotFoundException)
            {
                var directoryPath = _webRootPath + Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                await using var fileStream = new FileStream(_webRootPath + path, FileMode.Create);
                await content.CopyToAsync(fileStream);
            }
        }

        public async Task WriteFileAsync(string path, IFormFile file)
        {
            try
            {
                await using var fileStream = new FileStream(_webRootPath + path, FileMode.Create);
                await file.CopyToAsync(fileStream);
            }
            catch (DirectoryNotFoundException)
            {
                var directoryPath = _webRootPath + Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                await using var fileStream = new FileStream(_webRootPath + path, FileMode.Create);
                await file.CopyToAsync(fileStream);
            }
        }

        public async Task WriteFilesAsync(IEnumerable<string> path, IFormFileCollection files)
        {
            var pathAndFiles = path.Zip(files, (p, f) => new {Path = p, File = f});

            var directoryPath = _webRootPath + Path.GetDirectoryName(path.FirstOrDefault());

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            foreach (var item in pathAndFiles)
            {
                await using var fileStream =
                    new FileStream(_webRootPath + item.Path, FileMode.Create);
                await item.File.CopyToAsync(fileStream);
            }
        }
    }
}