using AutoMapper;
using Ntier.BLL.Interfaces;
using Ntier.DAL.Entities;
using Ntier.DAL.Interfaces;
using Ntier.DAL.Repositories;
using Ntier.DTO.DTO.Products;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.BLL.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IMapper _mapper;
        public ProductsService( IProductsRepository productsRepository , IMapper mapper ) { 
            _productsRepository = productsRepository;
            _mapper = mapper;
        }

        public async Task AddProductAsync(ProductToAddDTO productToAdd)
        {
             try
            {
               await _productsRepository.AddProductAsync(productToAdd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task UpdateProductAsync(ProductToAddDTO productToAdd )
        {
            await _productsRepository.UpdateProductAsync(productToAdd);
        }

        public async Task<ProductDTO> GetProductByIdAsync(string id)
        {
            try
            {
                var product = await _productsRepository.GetProductByIdAsync(id);
                var productDTO = new ProductDTO
                {
                    Id = product.Id,
                    name = product.Name,
                    CategoryId = product.CategoryId,
                    sale = product.Sale,
                    desc = product.Description,
                    stocks = _mapper.Map<ICollection<StockDTO>>(product.ProductSizeDetails),
                    pictures = _mapper.Map<ICollection<ImageDTO>>(product.ProductImages)
                };
                return productDTO;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProductsPagination> GetProductsAsync( ProductQueryParameters queryParameters )
        {
            try
            {
                var products = await _productsRepository.GetProductsAsync(queryParameters);
                List<ProductDTO> productDTOs = new List<ProductDTO>{};
                foreach ( var product in products )
                {
                    productDTOs.Add(new ProductDTO{
                        Id = product.Id,
                        name = product.Name,
                        CategoryId = product.CategoryId,
                        sale = product.Sale,
                        desc = product.Description,
                        stocks = _mapper.Map<ICollection<StockDTO>>(product.ProductSizeDetails),
                        pictures = _mapper.Map<ICollection<ImageDTO>>(product.ProductImages),
                    }); ;
                }
                var totalRow = await _productsRepository.GetQuantityProducts();
                ProductsPagination productsPagination = new ProductsPagination
                {
                    data = productDTOs ,
                    pagination = new Pagination
                    {
                        _page = queryParameters.page,
                        _limit = queryParameters.limit,
                        _totalRows = totalRow 
                    }
                };
                return productsPagination;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new ArgumentException("Some thing wrong");
            }
        }

        public async Task DeleteProductsAsync(string[] productsId)
        {
            await _productsRepository.DeleteProductsAsync(productsId);
        }
    }
}
