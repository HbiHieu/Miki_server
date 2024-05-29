using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ntier.DAL.Context;
using Ntier.DAL.Entities;
using Ntier.DTO.DTO;
using Ntier.DTO.DTO.Order;
using Ntier.DTO.DTO.Products;
using System.Data.SqlClient;

namespace Ntier.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ShopContext _shopContext;
        private readonly IConfiguration _configuration;
        public CartController(ShopContext shopContext, IConfiguration configuration)
        {
            _shopContext = shopContext;
            _configuration = configuration;
        }


        [HttpPost]

        public async Task<ActionResult> AddToCart(ProductCartDTO product)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("SQL");
                var item = await _shopContext.CartDetails.FirstOrDefaultAsync(item => item.ProductId == product.productID && item.SizeId == product.SizeId);
                if (item != null)
                {
                    item.Quantity = item.Quantity + 1;
                    await _shopContext.SaveChangesAsync();
                }
                else
                {
                    var query = @"INSERT INTO [dbo].[CART_DETAIL]
                                   ([CART_ID]
                                   ,[PRODUCT_ID]
                                   ,[QUANTITY]
                                   ,[PRICE]
                                   ,[USER_ID]
                                   ,[CREATE_AT]
                                   ,[SIZE_ID])
                             VALUES ";
                    query += $"( {product.cartID} , '{product.productID}', {product.quantity} , {product.price} , '{product.userId}', '{DateTime.Now}' , {product.SizeId} )";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using(SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = query;
                            command.ExecuteNonQuery();
                        }
                    } 
                    //await _shopContext.CartDetails.AddAsync(cartDetail);
                }
                return Ok("Thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetCartByUserID(string userID)
        {
            try
            {

                string connectionString = _configuration.GetConnectionString("SQL");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @$"select 
                                    CART_ID ,CART_DETAIL.PRODUCT_ID,[Name],CART_DETAIL.Quantity,Price,[User_ID],SIZE_ID,URL 
                                    from CART_DETAIL JOIN PRODUCT_IMAGE 
                                    ON CART_DETAIL.PRODUCT_ID = PRODUCT_IMAGE.PRODUCT_ID
                                    JOIN PRODUCT ON PRODUCT.ID = CART_DETAIL.PRODUCT_ID
                                    WHERE [USER_ID] = @UserID AND [INDEX] = 0";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userID);

                        var reader = await command.ExecuteReaderAsync();

                        if (!reader.HasRows)
                        {
                            return Ok(new List<CartDetailDTO>()); // Handle case where no order details found
                        }

                        var cartDetail = new CartDetailDTO();
                        var result = new List<CartDetailDTO>();

                        while (await reader.ReadAsync())
                        {
                            result.Add(new CartDetailDTO
                            {
                            CartID = (int)reader["CART_ID"],
                            Name = reader["Name"].ToString(),
                            ProductID = reader["PRODUCT_ID"].ToString(),
                            Quantity = (int)reader["Quantity"],
                            Price = (int)reader["Price"],
                            SizeId = (int)reader["SIZE_ID"],
                            URL = reader["URL"].ToString()
                        });
                        }
                        return Ok(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteProductInCart( string productId , string userId , int sizeID )
        {
            try
            {
                var query = @$"DELETE FROM CART_DETAIL WHERE PRODUCT_ID = '{productId}' AND USER_ID = '{userId}' AND SIZE_ID = {sizeID}";
                string connectionString = _configuration.GetConnectionString("SQL");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                }
                return Ok(new { message = "Thành công"});
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
