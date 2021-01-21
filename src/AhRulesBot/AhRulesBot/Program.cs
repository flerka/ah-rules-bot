using AhRulesBot.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AhRulesBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, configuration) => configuration.AddEnvironmentVariables(prefix: "AhRulesBot_"))
            .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    AppConfig options = configuration.Get<AppConfig>();
                    services.AddSingleton(options);

                    services.AddSerilogLogging();
                    services.AddTelegramBotClient();
                    services.AddAhRulesFile();
                    services.AddHostedService<Worker>();
                });
    }
}
