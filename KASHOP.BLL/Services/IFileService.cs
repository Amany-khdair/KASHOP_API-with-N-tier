using KASHOP.DAL.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public interface IFileService
    {
        Task<Result<FileUploadResult>> UploadAsync(IFormFile file);
        Task<Result<bool>> Delete(string publicId);
    }
}
