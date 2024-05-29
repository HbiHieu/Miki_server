using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ntier.BLL.Interfaces;
using Ntier.DTO.DTO.Products;
using Ntier.BLL.Attributes;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using Ntier.DTO.DTO;
using Ntier.DAL.Entities;
using System.Data.SqlClient;
using System.Reflection;

namespace Ntier.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        public ProductsController( IProductsService productsService , IWebHostEnvironment webHostEnvironment, IConfiguration configuration) { 
           _productsService = productsService;
           _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByIdAsync( string id)
        {
            try
            {
                var product = await _productsService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch(Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ExportExcel")]
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Template", "ThongKeSanPham.xlsx");
                FileInfo existingFile = new FileInfo(filePath);
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(existingFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int index = 5;
                    int jindex = index;

                    var data = await getProductsForExcel();
                    //var data = _mDetailPrice.GetItems(IDBaoGia);
                    worksheet.InsertRow(jindex, data.Count);

                    var type = typeof(ProductStatistical);
                    worksheet.Cells[$"A{jindex}"].LoadFromCollection(data, c =>
                    {
                        c.PrintHeaders = false;
                        c.Members = new MemberInfo[]
                        {
                            type.GetProperty("STT"),
                            type.GetProperty("Name"),
                            type.GetProperty("Sale"),
                            type.GetProperty("TenTheLoai"),
                            type.GetProperty("Create_At"),
                            type.GetProperty("Price"),
                            type.GetProperty("Quantity"),
                            type.GetProperty("StatusName")
                        };
                    });


                    jindex += data.Count;

                    // Add total row at the end
                    //worksheet.Cells[$"A{jindex}:F{jindex}"].Merge = true;
                    //worksheet.Cells[$"A{jindex}"].Value = "Tổng thanh toán";
                    //worksheet.Cells[$"A{jindex}"].Style.Font.Bold = true;

                    // Calculate total tax
                    //worksheet.Cells[$"G{jindex}"].Formula = $"=SUM(G6:G{jindex - 1})";

                    // Apply styles
                    worksheet.Cells[$"A{index}:H{jindex - 1}"].Style.WrapText = true;
                    worksheet.Cells[$"A{index}:H{jindex - 1}"].Style.Font.Size = 14;
                    worksheet.Cells[$"A{index}:H{jindex - 1}"].Style.Font.Name = "Times New Roman";
                    worksheet.Cells[$"A{index}:H{jindex - 1}"].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[$"A{index}:H{jindex - 1}"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[$"A{index}:H{jindex - 1}"].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[$"A{index}:H{jindex - 1}"].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    worksheet.Cells[$"A{index}:H{jindex - 1}"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells[$"A{index}:H{jindex - 1}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                  
                    package.Workbook.Properties.Title = "Export file excel";
                    package.Workbook.Properties.Author = "hieunt";


                    var fileContents = package.GetAsByteArray();


                    string filename = $"ThongKeSanPham_{DateTime.Now:ddMMyyyyhhmmss}.xlsx";
                    return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }    

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync( [FromQuery] ProductQueryParameters queryParameters)
        {
            try
            {
                var products = await _productsService.GetProductsAsync(queryParameters);
                return Ok(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> AddProductAsync( ProductToAddDTO productToAdd )
        {
            try
            {
                await _productsService.AddProductAsync(productToAdd);
                return Ok();
            }
            catch ( Exception ex )
            {
                return BadRequest( ex );
            }
        }

        [HttpPut("update")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> UpdateProductAsync( ProductToAddDTO productToAdd )
        {
            try
            {
                await _productsService.UpdateProductAsync(productToAdd);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("delete")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> DeleteProductAsync(string[] productsId )
        {
            try
            {
               await _productsService.DeleteProductsAsync(productsId);
                return Ok(new {
                    message = "Delete products successfully"
                });
            }
            catch(Exception ex) { 
                return BadRequest( ex.Message );
            }
        }

        private async Task<List<ProductStatistical>> getProductsForExcel()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("SQL");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @$"select ROW_NUMBER() OVER ( ORDER BY PRODUCT.ID ) as STT,PRODUCT.NAME as Name , Sale , 
                                CATEGORY.Name as TenTheLoai , Create_at , Min_price as Price , Quantity , 
                                IIF( [Status] = 1 , N'Khả dụng',N'Không khả dụng' ) as StatusName
                                FROM PRODUCT JOIN CATEGORY ON PRODUCT.CATEGORY_ID = CATEGORY.ID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        var reader = await command.ExecuteReaderAsync();

                        if (!reader.HasRows)
                        {
                            return new List<ProductStatistical>(); // Handle case where no order details found
                        }

                        var cartDetail = new ProductStatistical();
                        var result = new List<ProductStatistical>();

                        while (await reader.ReadAsync())
                        {
                            result.Add(new ProductStatistical
                            {
                                STT = Convert.ToInt64(reader["STT"]),
                                Name = reader["Name"].ToString(),
                                Sale = (int)reader["Sale"],
                                TenTheLoai = reader["TenTheLoai"].ToString(),
                                Create_At = reader["Create_at"].ToString(),
                                Price = (int)reader["Price"],
                                Quantity = (int)reader["Quantity"],
                                StatusName = reader["StatusName"].ToString()
                            });
                        }
                        return result;
                    }
                }
            }
            catch(Exception ex )
            {
                Console.Write(ex.Message);
                return new List<ProductStatistical>();
            }
        }    
    }
}
