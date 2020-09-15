using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;


namespace TodoApi
{
//warning code CS1591 is ignored, these code that shouldn't be exposed via the API docs 
#pragma warning disable CS1591
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                // WebHost.CreateDefaultBuilder 遵循 builder pattern，创建 a web host
                // The builder定义web server (比如UseKestrel) and the startup class (UseStartup).
                // kestrel web server默认将web host运行在IIS， 也可以在扩展方法中调用其他web server
                .UseStartup<Startup>();
    }
#pragma warning disable CS1591
}