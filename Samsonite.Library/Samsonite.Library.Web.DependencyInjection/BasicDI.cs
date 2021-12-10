using Microsoft.Extensions.DependencyInjection;
using Samsonite.Library.Business.Basic;

namespace Samsonite.Library.Web.DependencyInjection
{
    public class BasicDI
    {
        public static void Configure(IServiceCollection services)
        {
            //DI 注入
            //function
            services.AddScoped<IFunctionGroupService, FunctionGroupService>();
            services.AddScoped<IFunctionService, FunctionService>();
            //admin
            services.AddScoped<IRolesService, RolesService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<ILanguageService, LanguageService>();
            //config
            services.AddScoped<IConfigService, ConfigService>();
            services.AddScoped<IUploadConfigService, UploadConfigService>();
            services.AddScoped<IFtpConfigService, FtpConfigService>();
            services.AddScoped<IEmailGroupConfigService, EmailGroupConfigService>();
            //service
            services.AddScoped<IServiceConfigService, ServiceConfigService>();
            services.AddScoped<IServiceOperationLogService, ServiceOperationLogService>();
            services.AddScoped<IServiceLogService, ServiceLogService>();
            //api
            services.AddScoped<IApiConfigService, ApiConfigService>();
            services.AddScoped<IApiLogService, ApiLogService>();
            //log
            services.AddScoped<ISystemLogService, SystemLogService>();
        }
    }
}
