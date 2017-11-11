using System.IO;
using Microsoft.AspNetCore.Hosting;
using Digital_Content.DigitalContent.ExchangeRates;

namespace Digital_Content.DigitalContent.Web.Startup
{
    public class Program
    {
    

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
            
        }
    }
}
