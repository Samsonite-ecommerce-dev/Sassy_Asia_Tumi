using Microsoft.Extensions.DependencyInjection;
using Samsonite.Library.Core.Web;

namespace Samsonite.Library.DependencyInjection.Web
{
    public class CoreDI
    {
        public static void Configure(IServiceCollection services)
        {
            //DI 注入
            services.AddScoped<IAppConfigService, AppConfigService>();
            services.AddScoped<IAppLanguageService, AppLanguageService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IAppLogService, AppLogService>();
            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<IFtpService, FtpService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IUploadService, UploadService>();
        }
    }
}
