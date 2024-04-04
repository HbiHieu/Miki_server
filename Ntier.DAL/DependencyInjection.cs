using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ntier.DAL.Context;
using Ntier.DAL.Interfaces;
using Ntier.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntier.DAL
{
   public static class DependencyInjection
    {
     public static void RegisterDALDependencies( this IServiceCollection services , IConfiguration configuration)
    {
            services.AddDbContext<ShopContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SQL")) ;
            });
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
   }
   }
}
