using Microsoft.Extensions.DependencyInjection;
using Samsonite.Library.Business.WorkService;
using Samsonite.Library.Core.Web;
using Samsonite.Library.Core.WorkService;

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
