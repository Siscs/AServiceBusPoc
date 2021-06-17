using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Siscs.Api.ServiceBusTopicReceiver
{
    public class Program
    {
        public static void Main(string[] args)
        {

            if(args.Length <= 0)
            {
                Console.WriteLine("Informar o nome da subscription como parâmetro!");
                return;
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((hostContext , services) => {
                        services.AddSingleton<Parametros>(new Parametros 
                            { 
                                SubscriptionName = args[0]
                            });

                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
