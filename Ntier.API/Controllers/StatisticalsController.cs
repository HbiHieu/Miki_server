using Microsoft.AspNetCore.Mvc;
using Ntier.DAL.Context;
using Ntier.DTO.DTO;
using System.Data.SqlClient;

namespace Ntier.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticalsController : ControllerBase
    {
        private readonly ShopContext _shopContext;
        private readonly IConfiguration _configuration;
        public StatisticalsController(ShopContext shopContext, IConfiguration configuration)
        {
            _shopContext = shopContext;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> getRevenue()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("SQL");
                var statisticals = new List<StatisticalDTO>();
                var ov = new List<Overview>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // First query
                    string query = @"SELECT 
                             ORDER_ID, 
                             CONVERT(VARCHAR, ORDERS.CREATE_AT, 103) AS [Date],
                             SUM(MIN_PRICE * ORDER_DETAIL.QUANTITY) AS Revenue
                             FROM ORDER_DETAIL 
                             JOIN ORDERS ON ORDERS.ID = ORDER_DETAIL.ORDER_ID
                             JOIN PRODUCT ON PRODUCT_ID = PRODUCT.ID
                             WHERE STATUS_ID = 4
                             GROUP BY ORDER_ID, CONVERT(VARCHAR, ORDERS.CREATE_AT, 103)";

                    // Second query
                    string query2 = @"select STATUS_ORDER.Name , SODONHANG FROM STATUS_ORDER LEFT JOIN (
                                    select Name , COUNT(*) AS SODONHANG from STATUS_ORDER join 
                                    orders on 
                                    ORDERS.STATUS_ID = STATUS_ORDER.ID
                                    GROUP BY Name ) AS bt
                                    ON STATUS_ORDER.NAME = bt.NAME";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (!reader.HasRows)
                            {
                                return NotFound(); // Handle case where no order details found
                            }

                            while (await reader.ReadAsync())
                            {
                                statisticals.Add(new StatisticalDTO
                                {
                                    Date = reader["Date"].ToString(),
                                    Revenue = reader["Revenue"].ToString()
                                });
                            }
                        }
                    }

                    using (SqlCommand command = new SqlCommand(query2, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (!reader.HasRows)
                            {
                                return NotFound(); // Handle case where no order details found
                            }

                            while (await reader.ReadAsync())
                            {
                                ov.Add(new Overview()
                                {
                                    Name = reader["Name"].ToString(),
                                    soDonHang = reader.IsDBNull(reader.GetOrdinal("SODONHANG")) ? 0 : reader.GetInt32(reader.GetOrdinal("SODONHANG"))
                                }
                                );
                            }
                        }
                    }
                }

                return Ok(new { data = statisticals, overview = ov });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
