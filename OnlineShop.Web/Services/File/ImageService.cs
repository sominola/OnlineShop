using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OnlineShop.Data;
using OnlineShop.Data.Models;
using OnlineShop.Web.Extension;

namespace OnlineShop.Web.Services.File
{
    public class ImageService: IImageService
    {
        private readonly IFileService _fileService;
        private readonly AppDbContext _db;
        public ImageService(IFileService fileService, AppDbContext db)
        {
            _fileService = fileService;
            _db = db;
        }

        public async Task<ProductImage> UploadImageAsync(IFormFile file)
        {
            var idImage = Guid.NewGuid();
            
            var path = Config.PathToPhoto +  idImage + Path.GetExtension(file.FileName);
            
            await _fileService.WriteFileAsync(path, file);
            
            var image = new ProductImage
            {
                Name = file.FileName, 
                Path = path,
                Id = idImage
            };
            
            _db.Images.Add(image);
            return image;
        }

        public async Task<IEnumerable<ProductImage>> UploadImagesAsync(IFormFileCollection files)
        {
            // var listPaths = new List<string>(files.Count);
            var listImages = new List<ProductImage>(files.Count);
            
            foreach (var file in files)
            {
                var idImage = Guid.NewGuid();
                
                var path = Config.PathToPhoto + idImage + Path.GetExtension(file.FileName);
                
                // listPaths.Add(path);
                await _fileService.WriteFileAsync(path, file);

                
                var image = new ProductImage
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