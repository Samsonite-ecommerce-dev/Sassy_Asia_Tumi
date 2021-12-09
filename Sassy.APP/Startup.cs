using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Samsonite.Library.Core;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.DependencyInjection;

namespace Samsonite.Library.APP
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
            //������ϵע�����ݿ�������Ϣ
            services.AddDbContext<appEntities>(options => options.UseSqlServer(Configuration.GetConnectionString("appConnection")));
            services.AddDbContext<logEntities>(options => options.UseSqlServer(Configuration.GetConnectionString("logConnection")));

            //���HttpContext
            services.AddHttpContextAccessor();
            //���session
            services.AddSession();
            //�Զ���DIע��
            CoreDI.Configure(services);
            BusinessDI.Configure(services);
            WebInterfaceDI.Configure(services);
            //ȫ�ֹ�����ע��
            GlobalFilterDI.Configure(services);
            ////�Զ���Antiforgery
            ///MVC���Զ���<Form>��ǩ��������__RequestVerificationToken
            //services.AddAntiforgery(option =>
            //{

            //});

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            //services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAppConfigService appConfigService, IAppLanguageService appLanguageService)
        {
            //����վ�������ļ�
            appConfigService.LoadConfigCache();
            //����վ�����԰�
            appLanguageService.LoadLanguagePacks();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //����session
            app.UseSession();
            //���þ�̬�ļ��м��(wwwĿ¼)
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
