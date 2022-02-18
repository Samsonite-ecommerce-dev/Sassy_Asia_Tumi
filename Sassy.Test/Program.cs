using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Samsonite.Library.Business.Web.Basic.Models;
using Samsonite.Library.Business.WorkService;
using Samsonite.Library.Data.Entity.Models;
using Samsonite.Library.DependencyInjection.Web;
using Samsonite.Library.Utility;
using Samsonite.Library.Core.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

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
            (new TestApi()).TestSAS();

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
            services.AddDbContext<appEntities>(o => o.UseSqlServer(connectionStrings),ServiceLifetime.Transient);

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

            //Console.WriteLine(JsonSerializer.Serialize(dc));

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


            ////string x = "[{\"index\":0,\"value\":\"127.0.0.1\"},{\"index\":1,\"value\":\"::1\"},{\"index\":2,\"value\":\"1321\"}]";
            //string x = "[{\"index\":\"0\",\"value\":\"127.0.0.1\"},{\"index\":\"1\",\"value\":\"::1\"}]";
            //var y=JsonSerializer.Deserialize<List<MailAddressesAttr>>(x);
            //foreach(var item in y)
            //{
            //    Console.WriteLine($"{item.Index}-{item.Value}");
            //}

            System.Timers.Timer objTimer = new System.Timers.Timer();
            objTimer.Enabled = true;
            objTimer.Interval = 1000 * 10;
            objTimer.Elapsed += DoWork;

            Console.WriteLine("finish!");
        }

        private static void DoWork(object sender, EventArgs e)
        {
            ServiceModuleInfo objServiceModuleInfo = _appEntities.ServiceModuleInfo.AsNoTracking().Where(p => p.ModuleID == 1).SingleOrDefault();
            if (objServiceModuleInfo != null)
            {
                Console.WriteLine($"{DateTime.Now}:{objServiceModuleInfo.ModuleTitle}");
            }
        }

        private static AntiforgeryTokenModel Deserialize(string serializedToken)
        {
            AntiforgeryTokenModel _result = new AntiforgeryTokenModel();
            try
            {
                string _token = AESEncryption.Decrypt(serializedToken);
                string[] _arrayToken = _token.Split(".");
                _result.HeaderInfo = JsonSerializer.Deserialize<AntiforgeryTokenModel.Header>(EncryptHelper.DecodeBase64(_arrayToken[0]));
                _result.SecurityInfo = JsonSerializer.Deserialize<AntiforgeryTokenModel.Security>(EncryptHelper.DecodeBase64(_arrayToken[1]));
                _result.PayloadInfo = JsonSerializer.Deserialize<AntiforgeryTokenModel.Payload>(EncryptHelper.DecodeBase64(_arrayToken[2]));
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
