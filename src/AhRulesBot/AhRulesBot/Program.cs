using AhRulesBot.BotRequestsProcessing;
using AhRulesBot.BotRequestsProcessing.Interfaces;
using AhRulesBot.CustomRulesImport;
using AhRulesBot.Infrastructure;
using AhRulesBot.Infrastructure.ServiceCollectionExtensions;
using AhRulesBot.OldMessagesProcessing;
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
                    services.AddSingleton(options)
                            .AddMessageHandlers()
                            .AddSingleton<ITelegramMessageProcessor, TelegramMessageProcessor>()
                            .AddSingleton<IMessageValidator, MessageValidator>()
                            .AddSerilogLogging()
                            .AddTelegramBotClient()
                            .AddAhRulesFile()
                            .AddCardsFile()
                            .AddMsgToRemoveChannel()
                            .AddCustomRulesChannel()
                            .AddRulesDriveService()
                            .AddHostedService<OldMessagesWorker>()
                            .AddHostedService<BotRequestsWorker>()
                            .AddHostedService<CustomRulesWorker>();
                });
    }
}
