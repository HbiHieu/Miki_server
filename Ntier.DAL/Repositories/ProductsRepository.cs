using Microsoft.EntityFrameworkCore;
using Ntier.DAL.Context;
using Ntier.DAL.Entities;
using Ntier.DAL.Interfaces;
using Ntier.DTO.DTO.Products;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DAL.Repositories
{
    public class ProductsRepository :IProductsRepository
    {
        private readonly ShopContext _context;
        public ProductsRepository( ShopContext context ) { 
            _context = context;
        }

        public async Task AddProductSizeDetailAsync( ICollection<StockDTO> stocks , string productId )
        {
            try
            {
                int index = 0;
                string values = "" , space = "";
                    foreach (StockDTO stock in stocks)
                    {
                    if (index != 0)
                    {
                        space = ",";
                    }
                    values += $"{space}('{productId}',{stock.sizeId},{stock.quantity},{stock.Price})";
                    index++;
                    }
                string query = $@"insert into product_size_detail (product_id,size_id,quantity,price) values {values}";
                var result = await _context.Database.ExecuteSqlRawAsync(query);
            }
            catch ( Exception ex )
            {
                Console.WriteLine( ex );
            }
        }

        public async Task UpdateProductSizeDetailAsync(ICollection<StockDTO> stocks, string productId)
        {
            try
            {
                foreach (StockDTO stock in stocks)
                {
                    string query = $@"DELETE FROM PRODUCT_SIZE_DETAIL WHERE PRODUCT_ID = '{productId}' AND SIZE_ID = {stock.sizeId} ";
                    await _context.Database.ExecuteSqlRawAsync(query);
                }
                await AddProductSizeDetailAsync(stocks, productId);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddProductAsync(ProductToAddDTO productToAdd)
        {
            try
            {
                var product = new Product{
                    Id = productToAdd.Id ,
                    Name = productToAdd.name ,
                    Sale = productToAdd.sale ,
                    Description = productToAdd.desc ,
                    CategoryId = productToAdd.categoryId ,
                    MinPrice = productToAdd.MinPrice ,
                    CreateAt = DateTime.Now ,
                };
                await _context.Products.AddAsync( product );
                await _context.SaveChangesAsync();
                await AddProductSizeDetailAsync(productToAdd.stock , productToAdd.Id);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateProductAsync(ProductToAddDTO productToAdd)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(product => product.Id == productToAdd.Id);
                product.Name = productToAdd.name;
                product.Sale = productToAdd.sale;
                product.Description = productToAdd.desc;
                product.CategoryId = productToAdd.categoryId;
                product.MinPrice = productToAdd.MinPrice; 
                product.CreateAt = DateTime.Now;
                await _context.SaveChangesAsync();
                await UpdateProductSizeDetailAsync(productToAdd.stock, productToAdd.Id);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<int> GetQuantityProducts()
        {
            try
            {
                var result = await _context.Products.CountAsync();
                return result;
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message);
            }
        }

        public async Task<ICollection<Product>?> GetProductsAsync(ProductQueryParameters queryParameters)
        {
            try
            {
                List<Product> products = new List<Product>();
                Func<Product, object> orderBy = p => p.Name;
                if (queryParameters.sortBy == "price")
                {
                    orderBy = p => p.MinPrice;
                }
                if (queryParameters.order == "desc")
                {
                    products = _context.Products.OrderByDescending(orderBy).Skip( (queryParameters.page - 1) * queryParameters.limit).Take(queryParameters.limit).ToList();
                }
                else
                {
                products = _context.Products.OrderBy(orderBy).Skip((queryParameters.page - 1) * queryParameters.limit).Take(queryParameters.limit).ToList();
                }
                foreach (var product in products)
                {
                    _context.Entry(product)
                        .Collection(p => p.ProductImages)
                        .Load();
                    _context.Entry(product)
                        .Collection(p => p.ProductSizeDetails)
                        .Load();
                }
                return products;
            }
            catch (Exception ex) {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<Product> GetProductByIdAsync(string productId)
        {
            try
            {
                var product = await _context.Products.SingleOrDefaultAsync( product => product.Id == productId );
                _context.Entry(product)
                        .Collection(p => p.ProductImages)
                        .Load();
                _context.Entry(product)
                    .Collection(p => p.ProductSizeDetails)
                    .Load();
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteProductsAsync(string[] productsId)
        {
            try
            {
                foreach( var productId in productsId )
                {
                    await _context.Database.ExecuteSqlRawAsync($@"DELETE FROM PRODUCT_SIZE_DETAIL WHERE PRODUCT_ID = '{productId}'");
                    await _context.Database.ExecuteSqlRawAsync($@"DELETE FROM PRODUCT WHERE ID = '{productId}'");
                }    
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
