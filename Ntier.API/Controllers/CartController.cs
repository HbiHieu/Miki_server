using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ntier.DAL.Context;
using Ntier.DAL.Entities;
using Ntier.DTO.DTO.Products;

namespace Ntier.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        //private readonly ShopContext _shopContext;
        //public CartController ( ShopContext shopContext ) { 
        //    _shopContext = shopContext;
        //}


        //[HttpPost]

        //public async Task<ActionResult> AddToCart( ProductDTO product , string userId )
        //{
        //    try
        //    {
        //        var item = await _shopContext.CartDetails.FirstOrDefaultAsync( item => item.ProductId == product.Id );
        //        if (item != null) { 
        //            item.Quantity = item.Quantity + 1;
        //            await _shopContext.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            var cart = new Cart{
        //                CreateAt = DateTime.Now.ToString() ,
        //                UserId = userId
        //            };
        //            await _shopContext.Carts.AddAsync(cart);
        //            var cartDetail = new CartDetail
        //            {
        //                CartId = cart.Id,
        //                ProductId = product.Id,
        //                Quantity = product.,
        //                UserId = userId,
        //                CreateAt = DateTime.Now.ToString(),
        //            };
        //            await _shopContext.CartDetails.AddAsync(cartDetail);                   
        //        }
        //        return Ok("Thành công");
        //    }   
        //    catch ( Exception ex )
        //    {
        //        return BadRequest( ex.Message );
        //    }
        //}

        //[HttpGet]
        //public async Task<ActionResult> GetCartByUserID ( string userID )
        //{
        //    try
        //    {
        //        var cart = await _shopContext.CartDetails.Where(item => item.UserId == userID).GroupBy(item => item.ProductId).ToListAsync();
        //        return Ok( cart );
        //    }
        //   catch ( Exception ex )
        //    {
        //        return BadRequest(ex);
        //    }
        //}
    }
}
