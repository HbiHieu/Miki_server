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
using System.Text.RegularExpressions;
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
                    Status = 1
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
                    products = _context.Products.OrderByDescending(orderBy).Skip( (queryParameters.page - 1) * queryParameters.limit).Take(queryParameters.limit).Where(item => item.Status == 1 && item.Name.Contains(queryParameters.keySearch)).ToList();
                }
                else
                {
                    var productsQuery = _context.Products
                                    .Where(item => item.Status == 1);

                    var productList = await productsQuery.ToListAsync();

                    //Chuẩn hóa keySearch và lọc các sản phẩm 
                    var keyToSearchNormalized = Regex.Replace(queryParameters.keySearch.ToLower(), @"\s+", " ");
                    var filteredProducts = productList.Where(item =>
                    {
                        var itemNameNormalized = Regex.Replace(item.Name.ToLower(), @"\s+", " ");
                        return itemNameNormalized.Contains(keyToSearchNormalized);
                    }).ToList();

                    IOrderedEnumerable<Product> sortedProducts;
                    if (queryParameters.sortBy == "price")
                    {
                        sortedProducts = queryParameters.order == "desc" ?
                                         filteredProducts.OrderByDescending(p => p.MinPrice) :
                                         filteredProducts.OrderBy(p => p.MinPrice);
                    }
                    else 
                    {
                        sortedProducts = queryParameters.order == "desc" ?
                                         filteredProducts.OrderByDescending(p => p.Name) :
                                         filteredProducts.OrderBy(p => p.Name);
                    }

                    products = sortedProducts
                                        .Skip((queryParameters.page - 1) * queryParameters.limit)
                                        .Take(queryParameters.limit)
                                        .ToList();
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
                var product = await _context.Products.SingleOrDefaultAsync( product => product.Id == productId && product.Status == 1 );
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
                    //await _context.Database.ExecuteSqlRawAsync($@"DELETE FROM PRODUCT_SIZE_DETAIL WHERE PRODUCT_ID = '{productId}'");
                    //await _context.Database.ExecuteSqlRawAsync($@"DELETE FROM PRODUCT WHERE ID = '{productId}'");
                    await _context.Database.ExecuteSqlRawAsync($@"UPDATE PRODUCT SET [STATUS] = 0 WHERE ID = '{productId}'");
                }    
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
