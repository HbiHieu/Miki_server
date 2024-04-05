using Ntier.DTO.DTO.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.BLL.Interfaces
{
    public interface IImageService
    {
        public Task AddImageAsync(ICollection<ImageDTO> images);

        public Task DeleteImagesAsync(ImageDTO[] imagesDto );
    }
}
