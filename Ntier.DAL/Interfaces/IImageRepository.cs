using Ntier.DAL.Entities;
using Ntier.DTO.DTO.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DAL.Interfaces
{
    public interface IImageRepository
    {
        public Task AddImagesAsync(ICollection<ProductImage> images);
        public Task DeleteImagesAsync( ImageDTO[] imagesDto);
    }
}
