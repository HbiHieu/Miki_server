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
        private readonly IConfiguration _configuration;
        public OrderController ( ShopContext shopContext , IConfiguration configuration ) {
            _shopContext  = shopContext;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult> GetOrderByUserID ( string UserID )
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("SQL"); 

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
                    MIN_PRICE AS COST ,
                    (SELECT Name from STATUS_ORDER Where o.STATUS_ID = STATUS_ORDER.ID) as [Status]
                FROM ORDERS o
                INNER JOIN ORDER_DETAIL od ON o.ID = od.ORDER_ID
                INNER JOIN PRODUCT p ON od.PRODUCT_ID = p.ID
                LEFT JOIN PRODUCT_IMAGE pi ON p.ID = pi.PRODUCT_ID  
                WHERE o.USER_ID = @UserID AND [Index] = 0
                ORDER BY ID
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
                        var listOrderDetail = new List<OrderDetail_DTO>();
                        var products = new List<ProductOrderDTO>();

                        int oldOrderID = -1;
                        while (await reader.ReadAsync())
                        {
                            if ( (int)reader["ID"] != oldOrderID)
                            {
                                orderDetail.Products = products;
                                listOrderDetail.Add(orderDetail);
                                products = new List<ProductOrderDTO>();
                                orderDetail = new OrderDetail_DTO();
                                orderDetail.Id = (int)reader["ID"];
                                orderDetail.Address = reader["Address"].ToString();
                                orderDetail.CreateAt = reader["Create_At"].ToString(); // Assuming Create_At is DateTime
                                orderDetail.PhoneNumber = reader["PhoneNumber"].ToString();
                                orderDetail.UserId = reader["USERID"].ToString();
                                orderDetail.Status = reader["Status"].ToString();
                            }
                            products.Add(new ProductOrderDTO
                            {
                                ProductID = reader["ProductID"].ToString(),
                                Name = reader["Name"].ToString(),
                                Price = Convert.ToInt32(reader["COST"]),
                                Url = reader["Picture"].ToString(),
                                Quantity = Convert.ToInt32(reader["QUANTITY"]),
                                SizeId = 0 ,
                            });
                            oldOrderID = (int)reader["ID"];
                        }
                        orderDetail.Products = products;
                        listOrderDetail.Add(orderDetail);
                        listOrderDetail.RemoveAt(0);
                        return Ok(listOrderDetail);
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
                return Ok(new {message = "Thành công" });
            }     
            catch( Exception ex ){
                return Ok(new { message = "Thành công" });
            }
        }

        [HttpGet("GetOrders")]
        public async Task<ActionResult> GetAllOrders(int pageSize, int pageIndex)
        {
            try
            {   
                var total = await _shopContext.Orders.CountAsync();
                var orders = await _shopContext.Orders.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                return Ok(new
                {
                    data = orders,
                    pagination = new
                    {
                        _totalRows = total,
                        _page = pageIndex,
                        _limit = pageSize
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CancleOrder")]
        public async Task<ActionResult> CancleOrder( int orderId )
        {
            try
            {
                var order = await _shopContext.Orders.FirstOrDefaultAsync( item => item.Id == orderId );
                order.StatusId = 3;
                await _shopContext.SaveChangesAsync();
                return Ok(new { message = "Hủy thành công" });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ApproveOrder")]
        public async Task<ActionResult> ApproveOrder(int orderId)
        {
            try
            {
                var order = await _shopContext.Orders.FirstOrDefaultAsync(item => item.Id == orderId);
                order.StatusId = 2;
                await _shopContext.SaveChangesAsync();
                return Ok(new { message = "Duyệt thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ReceiveOrder")]
        public async Task<ActionResult> ReceiveOrder( int orderId )
        {
            try
            {
                var order = await _shopContext.Orders.FirstOrDefaultAsync(item => item.Id == orderId);
                order.StatusId = 4;
                await _shopContext.SaveChangesAsync();
                return Ok(new { message = "Thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
