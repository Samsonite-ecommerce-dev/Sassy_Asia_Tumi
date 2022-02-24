using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Core.Web;
using Samsonite.Library.DependencyInjection.Web;
using Samsonite.Library.DependencyInjection.WebApi;

namespace Sassy.APP
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //依赖关系注入数据库配置信息
            services.AddDbContext<appEntities>(options => options.UseSqlServer(Configuration.GetConnectionString("appConnection")));
            services.AddDbContext<logEntities>(options => options.UseSqlServer(Configuration.GetConnectionString("logConnection")));

            //添加HttpContext
            services.AddHttpContextAccessor();
            //添加session
            services.AddSession();
            //自定义DI注入
            CoreDI.Configure(services);
            BasicDI.Configure(services);
            CustomDI.Configure(services);
            WebApiDI.Configure(services);
            //全局过滤器注入
            GlobalFilterDI.Configure(services);
            ////自定义Antiforgery
            ///MVC会自动在<Form>标签下面生成__RequestVerificationToken
            //services.AddAntiforgery(option =>
            //{

            //});

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    //返回的JSON忽略NULL
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            //services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAppConfigService appConfigService, IAppLanguageService appLanguageService)
        {
            //加载站点配置文件
            appConfigService.LoadConfigCache();
            //加载站点语言包
            appLanguageService.LoadLanguagePacks();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //开启session
            app.UseSession();
            //配置静态文件中间件(www目录)
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseCookiePolicy();
            //app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
