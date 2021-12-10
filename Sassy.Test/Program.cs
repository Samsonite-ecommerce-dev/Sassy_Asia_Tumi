using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Samsonite.Library.Business.Custom;
using Samsonite.Library.Web.Core;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.Web.DependencyInjection;
using Samsonite.Library.WebApi.DependencyInjection;
using Samsonite.Library.Utility;
using System;
using System.IO;
using System.Text;

namespace Samsonite.Sassy.Test
{
    class Program
    {
        private static IBaseService _baseService;
        private static ISAPService iSAPService;
        private static IAntiforgeryService _antiforgeryService;
        private static appEntities _appEntities;
        static void Main(string[] args)
        {
            Init();

            //TestBug();

            //TestApi
            TestApi.TestSAS();

            Console.ReadLine();
        }

        private static void Init()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", false, true);
            //.AddEnvironmentVariables();

            var configurationRoot = builder.Build();
            var connectionStrings = configurationRoot.GetSection("ConnectionStrings").GetSection("appConnection").Value;

            var services = new ServiceCollection();
            services.AddDbContext<appEntities>(o => o.UseSqlServer(connectionStrings));

            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddSingleton<IHostEnvironment>(new HostingEnvironment() { });
            //注入自定义类
            CoreDI.Configure(services);
            CustomDI.Configure(services);

            //构建容器
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            _baseService = serviceProvider.GetService<IBaseService>();
            _antiforgeryService = serviceProvider.GetService<IAntiforgeryService>();
            iSAPService = serviceProvider.GetService<ISAPService>();
            _appEntities = serviceProvider.GetService<appEntities>();


        }

        private static void TestBug()
        {
            //IDictionary<string, string> dc = new Dictionary<string, string>();
            //dc.Add("t1","111");
            //dc.Add("t2", "222");
            //dc.Add("t3", "333");

            //Console.WriteLine(JsonHelper.JsonSerialize(dc));

            //var x = iSAPService.DownMaterial();
            ////分页批量保存
            //decimal pageSize = 10;
            ////处理Line
            //int totalPage = (int)Math.Ceiling(0 / pageSize);
            //Console.WriteLine(totalPage.ToString());
            //for (int page = 0; page < totalPage; page++)
            //{
            //    Console.WriteLine(page.ToString());
            //}


            StringBuilder _s = new StringBuilder();
            for(var i = 0; i < 10; i++)
            {
                _s = new StringBuilder();
                _s.Append(i.ToString());
                Console.WriteLine(_s.ToString());
            }
            
            Console.WriteLine("finish!");
        }

        private static AntiforgeryTokenModel Deserialize(string serializedToken)
        {
            AntiforgeryTokenModel _result = new AntiforgeryTokenModel();
            try
            {
                string _token = AESEncryption.Decrypt(serializedToken);
                string[] _arrayToken = _token.Split(".");
                _result.HeaderInfo = JsonHelper.JsonDeserialize<AntiforgeryTokenModel.Header>(EncryptHelper.DecodeBase64(_arrayToken[0]));
                _result.SecurityInfo = JsonHelper.JsonDeserialize<AntiforgeryTokenModel.Security>(EncryptHelper.DecodeBase64(_arrayToken[1]));
                _result.PayloadInfo = JsonHelper.JsonDeserialize<AntiforgeryTokenModel.Payload>(EncryptHelper.DecodeBase64(_arrayToken[2]));
                if (_arrayToken.Length >= 4)
                {
                    _result.Signature = _arrayToken[3];
                }
            }
            catch
            {
                _result = null;
            }
            return _result;
        }
    }
}
