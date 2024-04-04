using Ntier.DAL.Entities;
using Ntier.DTO.DTO.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.BLL.Interfaces
{
    public interface IProductsService
    {
        public Task<ProductDTO> GetProductByIdAsync(string id);
        public Task AddProductAsync(ProductToAddDTO productToAdd);
        public Task UpdateProductAsync(ProductToAddDTO productToAdd);
        public Task<ProductsPagination> GetProductsAsync( ProductQueryParameters queryParameters );
        public Task DeleteProductsAsync(string[] productsId);
    }
}
