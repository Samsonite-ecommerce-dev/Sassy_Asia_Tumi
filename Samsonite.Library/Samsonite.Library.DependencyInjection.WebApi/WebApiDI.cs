﻿using Microsoft.Extensions.DependencyInjection;
using Samsonite.Library.Bussness.WebApi;
using Samsonite.Library.Core.WebApi;

namespace Samsonite.Library.DependencyInjection.WebApi
{
    public class WebApiDI
    {
        public static void Configure(IServiceCollection services)
        {
            //core
            services.AddScoped<IApiBaseService, ApiBaseService>();
            services.AddScoped<IAuthorizeService, AuthorizeService>();
            services.AddScoped<IMenuService, MenuService>();
            //api
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISparePartService, SparePartService>();
        }
    }
}
