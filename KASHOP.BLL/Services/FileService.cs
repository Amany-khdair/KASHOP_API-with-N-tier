using KASHOP.DAL.Dto;
using Microsoft.AspNetCore.Http;
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
        public async Task<Result<string>> UploadAsync(IFormFile file)
        {
            try
            {
                if (file is null || file.Length < 0)
                {
                    return new Result<string>
                    {
                        Success = false,
                        Message = "No file was provided"
                    };
                }

                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                {
                    return new Result<string>
                    {
                        Success = false,
                        Message = $"File type {extension} is not allowed!"
                    };
                }

                if (file.Length > _maxFileSize)
                {
                    return new Result<string>
                    {
                        Success = false,
                        Message = $"File size exceeds the maximum limit of {_maxFileSize / (1024 * 1024)} MB."
                    };
                }
                var fileName = Guid.NewGuid().ToString() + extension;

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
                return new Result<string>
                {
                    Success = true,
                    Message = "File uploaded successfully",
                    Data = fileName
                };
            }
            catch (Exception ex)
            {
                return new Result<string>
                {
                    Success = false,
                    Message = $"An error occurred while uploading the file: {ex.InnerException.Message}"
                };
            }
        }

       
    }
}
