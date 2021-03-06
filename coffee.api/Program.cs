using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace coffee.api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>().UseUrls("http://0.0.0.0:8080");
            });
        }

        // WILO: when descaling, it should be in active mode and not alert.
    }
}
