using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OnlineShop.Data;
using OnlineShop.Data.Models;

namespace OnlineShop.Services.File
{
    public class ImageService : IImageService
    {
        private readonly IFileService _fileService;
        private readonly AppDbContext _db;
        private HttpClient _client;
        private readonly string _pathToPhoto;
        
        public ImageService(IFileService fileService, AppDbContext db)
        {
            _fileService = fileService;
            _db = db;
            _pathToPhoto = Config.PathToPhoto;
        }


        public async Task<SiteImage> UploadImageAsync(IFormFile file)
        {
            var idImage = Guid.NewGuid();

            var path = _pathToPhoto + idImage + Path.GetExtension(file.FileName);

            await _fileService.WriteFileAsync(path, file);

            var image = new SiteImage
            {
                Name = file.FileName,
                Path = path,
                Id = idImage
            };

            _db.Images.Add(image);
            return image;
        }

        public async Task<IEnumerable<SiteImage>> UploadImagesFromWebUrlAsync(List<string> urls)
        {
            var listImages = new List<SiteImage>(urls.Count);
            foreach (var url in urls)
            {
                
                var idImage = Guid.NewGuid();

                const string filename = "FromWebImage!!";
                var image = new SiteImage
                {
                    Name = filename,
                    Path = url,
                    Id = idImage
                };

                listImages.Add(image);
                
            }
            await _db.AddRangeAsync(listImages);
            await _db.SaveChangesAsync();

            return listImages;
        }

        public async Task<IEnumerable<SiteImage>> UploadImagesFromWebAsync(List<string> urls)
        {
            var listImages = new List<SiteImage>(urls.Count);
            
            _client ??= new HttpClient();
            foreach (var url in urls)
            {
                var file = await _client.GetAsync(url);
                
                var idImage = Guid.NewGuid();

                var nameFile = file.Content.Headers.GetValues("content-disposition").FirstOrDefault();
                var filename = new ContentDisposition(nameFile).FileName;
                var path =_pathToPhoto + idImage + Path.GetExtension(filename);

                await _fileService.WriteFilesFromStreamAsync(path, file.Content);

                var image = new SiteImage
                {
                    Name = filename,
                    Path = path,
                    Id = idImage
                };

                listImages.Add(image);
                
            }
            await _db.AddRangeAsync(listImages);
            await _db.SaveChangesAsync();

            return listImages;
        }

        public async Task<IEnumerable<SiteImage>> UploadImagesAsync(IFormFileCollection files)
        {
            // var listPaths = new List<string>(files.Count);
            var listImages = new List<SiteImage>(files.Count);

            foreach (var file in files)
            {
                var idImage = Guid.NewGuid();

                var path =_pathToPhoto + idImage + Path.GetExtension(file.FileName);

                // listPaths.Add(path);
                await _fileService.WriteFileAsync(path, file);


                var image = new SiteImage
                {
                    Name = file.FileName,
                    Path = path,
                    Id = idImage
                };

                listImages.Add(image);
            }
            // await _fileService.WriteFilesAsync(listPaths, files);

            await _db.AddRangeAsync(listImages);
            await _db.SaveChangesAsync();

            return listImages;
        }
    }
}