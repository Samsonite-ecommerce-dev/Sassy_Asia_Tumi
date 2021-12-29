using Microsoft.Extensions.DependencyInjection;
using Samsonite.Library.Business.WorkService;
using Samsonite.Library.Web.Core;
using Samsonite.Library.WorkService.Core;

namespace Samsonite.Library.DependencyInjection.WorkService
{
    public class WorkServiceDI
    {
        public static void Configure(IServiceCollection services)
        {
            //core
            services.AddScoped<IApplicationService, ApplicationService>();
            //service
            services.AddScoped<ISAPService, SAPService>();
            //related
            services.AddScoped<IFtpService, FtpService>();
        }
    }
}
