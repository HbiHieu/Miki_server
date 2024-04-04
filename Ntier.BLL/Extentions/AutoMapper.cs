using AutoMapper;
using Ntier.DAL.Entities;
using Ntier.DTO.DTO.Products;
using Ntier.DTO.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.BLL.Extentions
{
    public class AutoMapper : Profile
    {
        public AutoMapper() { 
            CreateMap<User,UserRegisterDTO>().ReverseMap();
            CreateMap<ProductImage, ImageDTO>().ReverseMap();
            CreateMap<ProductSizeDetail, StockDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
        }
    }
}
