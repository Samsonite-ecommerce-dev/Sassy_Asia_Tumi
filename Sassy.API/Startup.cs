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
            //������ϵע�����ݿ�������Ϣ
            services.AddDbContext<appEntities>(options => options.UseSqlServer(Configuration.GetConnectionString("appConnection")));
            services.AddDbContext<logEntities>(options => options.UseSqlServer(Configuration.GetConnectionString("logConnection")));
            //�Զ���DIע��
            WebApiDI.Configure(services);

            //�����������,ע��Ҫ���� AddMvc ֮ǰ
            services.AddCors();
            services.AddMvc(options =>
            {
                //Ȩ����֤
                options.Filters.Add(typeof(CustomAuthorizeFilter));
                //ȫ�ִ���
                options.Filters.Add(typeof(CustomExceptionFilter));
            }).AddJsonOptions(options =>
            {
                //���ص�JSON����NULL
                options.JsonSerializerOptions.IgnoreNullValues = true;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            //���Swagger
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

            //���Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tumi Sassy API v1");
            });
        }
    }
}
