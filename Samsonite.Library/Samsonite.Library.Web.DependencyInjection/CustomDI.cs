using Microsoft.Extensions.DependencyInjection;
using Samsonite.Library.Business.Custom;

namespace Samsonite.Library.Web.DependencyInjection
{
    public class CustomDI
    {
        public static void Configure(IServiceCollection services)
        {
            //DI 注入
            //sap
            services.AddScoped<ISAPService, SAPService>();
            //job
            services.AddScoped<IWorkerService, WorkerService>();
            //product
            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<IProductQueryService, ProductQueryService>();
            services.AddScoped<ISparePartQueryService, SparePartQueryService>();
            services.AddScoped<IUploadSparePartService, UploadSparePartService>();
        }
    }
}
