using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ntier.DAL.Context;
using Ntier.DAL.Entities;
using Ntier.DTO.DTO.Order;
using Ntier.DTO.DTO.Products;
using System.Data.SqlClient;

namespace Ntier.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ShopContext _shopContext;
        public OrderController ( ShopContext shopContext ) {
            _shopContext  = shopContext;
        }

        [HttpGet]
        public async Task<ActionResult> GetOrderByUserID ( string UserID )
        {
            try
            {
                string connectionString = "Server=DESKTOP-O1721LD\\HBI;Database=MIKI_SHOP;UID=sa;PWD=d11052003;"; // Replace with your actual connection string

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT 
                    o.ID, 
                    o.User_ID AS USERID, 
                    pi.URL AS Picture, 
                    o.Create_At, 
                    p.Name, 
                    o.Address, 
                    o.Phone_Number AS PhoneNumber,
                    od.PRODUCT_ID AS ProductID, 
                    od.QUANTITY,
                     MIN_PRICE AS COST
                FROM ORDERS o
                INNER JOIN ORDER_DETAIL od ON o.ID = od.ORDER_ID
                INNER JOIN PRODUCT p ON od.PRODUCT_ID = p.ID
                LEFT JOIN PRODUCT_IMAGE pi ON p.ID = pi.PRODUCT_ID  
                WHERE o.USER_ID = @UserID AND [Index] = 0
            ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", UserID);

                        var reader = await command.ExecuteReaderAsync();

                        if (!reader.HasRows)
                        {
                            return NotFound(); // Handle case where no order details found
                        }

                        var orderDetail = new OrderDetail_DTO();
                        var products = new List<ProductOrderDTO>();

                        while (await reader.ReadAsync())
                        {
                            orderDetail.Address = reader["Address"].ToString();
                            orderDetail.CreateAt = reader["Create_At"].ToString(); // Assuming Create_At is DateTime
                            orderDetail.PhoneNumber = reader["PhoneNumber"].ToString();
                            orderDetail.UserId = reader["USERID"].ToString();

                            products.Add(new ProductOrderDTO
                            {
                                ProductID = reader["ProductID"].ToString(),
                                Name = reader["Name"].ToString(),
                                Cost = Convert.ToInt32(reader["COST"]),
                                Picture = reader["Picture"].ToString(),
                                Quantity = Convert.ToInt32(reader["QUANTITY"]),
                                Size = 0 
                            });
                        }

                        orderDetail.Products = products;
                        return Ok(orderDetail);
                    }
                }
            }
            catch(Exception ex){ 
               return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddOrder")]
        public async Task<ActionResult> AddOrder( OrderDetail_DTO orderDTO )
        {   
            try {
                var order = new Order
                {
                    Address = orderDTO.Address,
                    CreateAt = DateTime.Now.ToString(),
                    PhoneNumber = orderDTO.PhoneNumber,
                    StatusId = 1,
                    UserId = orderDTO.UserId
                };
                await _shopContext.Orders.AddAsync( order );
                await _shopContext.SaveChangesAsync();
                List<OrderDetail> orders = new List<OrderDetail>();
                string str = "INSERT INTO ORDER_DETAIL VALUES ";
                foreach( var item in orderDTO.Products )
                {
                    str += $"({order.Id},'{item.ProductID}',{item.Quantity}),";                    
                }
                var strNew = str.Remove( str.Length - 1 );
                await _shopContext.OrderDetails.FromSqlRaw(strNew).ToListAsync();
                await _shopContext.SaveChangesAsync();
                return Ok("Thành công");
            }     
            catch( Exception ex ){
                return BadRequest(ex.Message);
            }
        }
    }
}
