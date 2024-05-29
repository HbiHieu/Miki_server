using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ntier.DAL.Context;
using Ntier.DAL.Entities;

namespace Ntier.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ShopContext _context;

        public CategoriesController(ShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories( int pageSize , int pageIndex )
        {
          if (_context.Categories == null)
          {
              return NotFound();
          }
          var categories = await _context.Categories.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(new
            {
                data = categories,
                pagination = new
                {
                    _totalRows = categories.Count,
                    _page = pageIndex,
                    _limit = pageSize
                }
            });
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
          if (_context.Categories == null)
          {
              return NotFound();
          }
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }
     

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
          try
            {
                if (_context.Categories == null)
                {
                    return Problem("Entity set 'ShopContext.Categories'  is null.");
                }
                if (category.Id == -1)
                {
                    var a = new Category { Name = category.Name };
                    await _context.Categories.AddAsync(a);
                }
                else
                {
                    var ca = await _context.Categories.FirstOrDefaultAsync(item => item.Id == category.Id);
                    ca.Name = category.Name;
                }
                await _context.SaveChangesAsync();
                return Ok(new { message = "Thành công" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            var isHaveProduct = await _context.Products.AnyAsync(e => e.CategoryId == id);
            if ( isHaveProduct )
            {
                return BadRequest(new { message = "Có sản phẩm đang sử dụng không thể xóa" });
            }    
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
