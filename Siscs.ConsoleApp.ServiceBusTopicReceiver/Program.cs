using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Siscs.ConsoleApp.ServiceBusTopicReceiver.Services;

namespace Siscs.ConsoleApp.ServiceBusTopicReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Inicializando ConsoleApp.....");
            Console.WriteLine("--------------------------------------------");

            if(args.Length <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine("Informe o SubscriptionName como parâmetro.");
                Console.WriteLine("--------------------------------------------");
                return;
            }
            
            CreateHostBuilder(args).Build().Run();

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"Listening Messages on subscription {args[0]} .....");
            Console.WriteLine("--------------------------------------------");
            Console.ReadKey();

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"Exiting......");
            Console.WriteLine("--------------------------------------------");
 
        }

         public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration((configurationBuilder) => {

                    //configurationBuilder.AddJsonFile("appsettings.Development.json", optional: true);

                    // ja adiciona os json files conforme variavel de ambiente
                    // DOTNET_ENVIRONMENT NO LAUCH
                    configurationBuilder.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(new Parametros {
                        SubscriptionName = args[0]
                    });

                    var configSection = hostContext.Configuration.GetSection("ServiceBus");
                    
                    // obj config
                    var serviceBusConfig = configSection.Get<ServiceBusConfig>();

                    // configure for addoptions
                    services.Configure<ServiceBusConfig>(configSection);

                    services.AddSingleton<MessageReceiverService>();

                    //services.AddHostedService<Worker>();
                    var serviceProvider = services.BuildServiceProvider();

                    var messageService = serviceProvider.GetService<MessageReceiverService>();
                    messageService.RegisterMessageHandler();

                });
                // .ConfigureAppConfiguration((hostingContext, config) => {
                
                //     var env = hostingContext.HostingEnvironment;

                //     config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                //     if (env.IsDevelopment() || env.EnvironmentName.ToLower() == "Development")
                //         config.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
                //     else if (env.IsStaging() || env.EnvironmentName.ToLower() == "Stage")
                //         config.AddJsonFile("appsettings.Stage.json", optional: true, reloadOnChange: true);                    

                //     config.AddEnvironmentVariables();

                // });

    }
}
