using Microsoft.Extensions.DependencyInjection;
using Samsonite.Library.Core;

namespace Samsonite.Library.APP
{
    public class GlobalFilterDI
    {
        public static void Configure(IServiceCollection services)
        {
            //权限DI注入
            services.AddScoped<UserLoginAuthorize>();
            services.AddScoped<UserPowerAuthorize>();
            //CSRF防御注入
            services.AddScoped<IAntiforgeryService, AntiforgeryService>();
            //全局异常DI注入
            services.AddScoped<GlobalExceptions>();
        }
    }
}