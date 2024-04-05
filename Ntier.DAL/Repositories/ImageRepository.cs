using Microsoft.EntityFrameworkCore;
using Ntier.DAL.Context;
using Ntier.DAL.Entities;
using Ntier.DAL.Interfaces;
using Ntier.DTO.DTO.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DAL.Repositories
{
    public class ImageRepository:IImageRepository
    {
        private readonly ShopContext _context;
        public ImageRepository( ShopContext context ) { 
            _context = context;
        }

        public async Task AddImagesAsync(ICollection<ProductImage> images)
        {
            try
            {
                await _context.ProductImages.AddRangeAsync(images);
                await _context.SaveChangesAsync();
            }
            catch ( Exception ex )
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task DeleteImagesAsync( ImageDTO[] imagesDto)
        {
            try
            {
                foreach (var imageDTO in imagesDto)
                {
                    await _context.Database.ExecuteSqlRawAsync($@"DELETE FROM PRODUCT_IMAGE WHERE PRODUCT_ID = '{imageDTO.ProductId}' AND [INDEX] = {imageDTO.Index}");
                }
            }
            catch(Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}
