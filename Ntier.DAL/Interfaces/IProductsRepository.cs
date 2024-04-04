using Ntier.DAL.Entities;
using Ntier.DTO.DTO.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DAL.Interfaces
{
    public interface IProductsRepository
    {
        public Task AddProductAsync(ProductToAddDTO productToAdd );
        public Task UpdateProductAsync(ProductToAddDTO productToAdd);
        public Task AddProductSizeDetailAsync( ICollection<StockDTO> stocks, string productId);
        public Task UpdateProductSizeDetailAsync( ICollection<StockDTO> stocks, string productId);
        public Task<ICollection<Product>?>GetProductsAsync( ProductQueryParameters queryParameters );

        public Task<int> GetQuantityProducts();

        public Task<Product> GetProductByIdAsync( string productId );

        public Task DeleteProductsAsync(string[] productsId);
    }
}
