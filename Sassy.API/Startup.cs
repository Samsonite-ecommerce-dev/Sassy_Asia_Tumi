using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.DependencyInjection.WebApi;

namespace Samsonite.Library.API
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
            //自定义DI注入
            WebApiDI.Configure(services);

            //允许跨域请求,注意要放在 AddMvc 之前
            services.AddCors();
            services.AddMvc(options =>
            {
                //权限验证
                options.Filters.Add(typeof(CustomAuthorizeFilter));
                //全局错误
                options.Filters.Add(typeof(CustomExceptionFilter));
            }).AddJsonOptions(options =>
            {
                //返回的JSON忽略NULL
                options.JsonSerializerOptions.IgnoreNullValues = true;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            //添加Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tumi Sassy", Version = "v1" });
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //添加Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tumi Sassy API v1");
            });
        }
    }
}
