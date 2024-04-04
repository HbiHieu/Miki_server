using Microsoft.Extensions.DependencyInjection;
using Ntier.BLL.Interfaces;
using Ntier.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.BLL
{
    public static class DependencyInjection
    {
        public static void RegisterBLLDependencies( this IServiceCollection services )
        {
            services.AddAutoMapper( typeof(Ntier.BLL.Extentions.AutoMapper) );
            services.AddScoped<IUserService,UserService>();
            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<IImageService, ImageService>();
        }
    }
}
