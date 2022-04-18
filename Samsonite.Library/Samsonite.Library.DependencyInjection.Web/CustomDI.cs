using Microsoft.Extensions.DependencyInjection;
using Samsonite.Library.Business.Web.Custom;

namespace Samsonite.Library.DependencyInjection.Web
{
    public class CustomDI
    {
        public static void Configure(IServiceCollection services)
        {
            //DI 注入

            //product
            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<IProductQueryService, ProductQueryService>();
            services.AddScoped<ISparePartGroupService, SparePartGroupService>();
            services.AddScoped<ISparePartQueryService, SparePartQueryService>();
            services.AddScoped<IUploadSparePartService, UploadSparePartService>();
        }
    }
}
