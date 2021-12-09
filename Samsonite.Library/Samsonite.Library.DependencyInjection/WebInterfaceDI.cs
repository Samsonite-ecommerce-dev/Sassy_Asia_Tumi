using Microsoft.Extensions.DependencyInjection;
using Samsonite.Library.WebApi;

namespace Samsonite.Library.DependencyInjection
{
    public class WebInterfaceDI
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddScoped<IApiService, ApiService>();
            //api
            services.AddScoped<ISASService, SASService>();
        }
    }
}
