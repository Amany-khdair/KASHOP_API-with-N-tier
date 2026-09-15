using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using KASHOP.DAL.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public class FileService : IFileService
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".svg"};
        private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB
        private readonly Cloudinary _cloudinary;
        public FileService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["CloudinarySettings:CloudName"],
                configuration["CloudinarySettings:ApiKey"],
                configuration["CloudinarySettings:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }
        public async Task<Result<FileUploadResult>> UploadAsync(IFormFile file)
        {          
            if (file is null || file.Length == 0)
            {
                return Result<FileUploadResult>.Fail("No file was provided");            
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                return Result<FileUploadResult>.Fail($"File type {extension} is not allowed!");                    
            }

            if (file.Length > _maxFileSize)
            {
                return Result<FileUploadResult>.Fail($"File size exceeds the maximum limit of {_maxFileSize / (1024 * 1024)} MB.");      
            }

            using(var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "KASHOP"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                
                if (uploadResult.Error != null)
                {
                    return Result<FileUploadResult>.Fail(uploadResult.Error.Message);                   
                }
                return Result<FileUploadResult>.Ok(new FileUploadResult
                {
                    Url = uploadResult.SecureUri.ToString(),
                    PublicId = uploadResult.PublicId
                }, "File uploaded successfully");
                //return new Result<FileUploadResult>
                //{
                //    Success = true,
                //    Message = "File uploaded successfully",
                //    Data = uploadResult.SecureUri.ToString()
                //};
            }

            //var fileName = Guid.NewGuid().ToString() + extension;

            //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);

            //using (var stream = System.IO.File.Create(filePath))
            //{
            //    await file.CopyToAsync(stream);
            //}
            //return Result<string>.Ok(fileName, "File uploaded successfully");
            ////return new Result<string>
            ////{
            ////    Success = true,
            ////    Message = "File uploaded successfully",
            ////    Data = fileName
            ////};            
        }

        //to remove a file from cloudinary, we need the public id of the file, which is returned when we upload the file
        public async Task<Result<bool>> Delete(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
            
            if (deletionResult.Result != "ok")
            {
                return Result<bool>.Fail($"Failed to delete file: {deletionResult.Error?.Message}");           
            }            
            return Result<bool>.Ok(true, "File deleted successfully");

        }
    }
}
