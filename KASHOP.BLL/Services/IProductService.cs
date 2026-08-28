using KASHOP.DAL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Services
{
    public interface IProductService
    {
        Task<Result<ProductResponse>> CreateProduct(ProductRequest request);
    }
}
