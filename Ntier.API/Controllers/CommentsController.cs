using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ntier.DAL.Context;
using Ntier.DAL.Entities;
using Ntier.DTO.DTO.Order;
using System.Data.SqlClient;

namespace Ntier.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ShopContext _shopContext;
        private readonly IConfiguration _configuration;
        public CommentsController(ShopContext shopContext, IConfiguration configuration)
        {
            _shopContext = shopContext;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetCommentByProductId( string productId ) { 
            try
            {
                string connectionString = _configuration.GetConnectionString("SQL");
                var query = @"SELECT COMMENT.ID , USER_ID ,RATING,Name, CONTENT , CREATE_AT
                              FROM COMMENT JOIN [USER] ON
                              COMMENT.USER_ID = [USER].ID
                              WHERE PRODUCT_ID = @ProductId
                             ";                    

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(query,connection))
                        {
                        command.Parameters.AddWithValue("@ProductId", productId);
                        var reader = await command.ExecuteReaderAsync();

                        if (!reader.HasRows)
                        {
                            return NotFound(); // Handle case where no order details found
                        }

                        var listComments = new List<CommentDTO>();
                        while (await reader.ReadAsync())
                        {
                            listComments.Add(new CommentDTO
                            {
                                Id = (int)reader["ID"] ,
                                UserId = reader["USER_ID"].ToString(),
                                Content = reader["CONTENT"].ToString(),
                                CreateAt = reader["CREATE_AT"].ToString() ,
                                Name = reader["Name"].ToString() ,
                                Rating = (int)reader["RATING"]
                            });
                        }
                        return Ok(new {data = listComments});
                    }
                }
            }
            catch(Exception e)
            {
                return Ok(new { data = new List<CommentDTO>()
            });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddComment( Comment comment )
        {
            try
            {
                comment.CreateAt = DateTime.Now.ToString();
                var comments = await _shopContext.Comments.AddAsync(comment);
                await _shopContext.SaveChangesAsync();
                return Ok(new { message = "Thêm thành công" });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteComment( int commentId )
        { 
            try
            {
                var comments = await _shopContext.Comments.FirstOrDefaultAsync( item => item.Id == commentId );
                if (comments != null)
                {
                    _shopContext.Comments.Remove(comments);
                    await _shopContext.SaveChangesAsync();
                }    
                return Ok(new { message = "Xóa thành công" });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
