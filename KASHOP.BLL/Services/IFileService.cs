using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    internal interface IFileService
    {
        Task<string?> UploadAsync(IFormFile file);
    }
}
