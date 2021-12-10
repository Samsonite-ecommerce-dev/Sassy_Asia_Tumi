using Microsoft.Extensions.DependencyInjection;
using Samsonite.Library.Bussness.WebApi;

namespace Samsonite.Library.WebApi.DependencyInjection
{
    public class WebApiDI
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddScoped<IApiService, ApiService>();
            //api
            services.AddScoped<ISASService, SASService>();
        }
    }
}
