using AhRulesBot.MessageProcessing;
using AhRulesBot.MessageProcessing.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Telegram.Bot;

namespace AhRulesBot.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddTelegramBotClient(this IServiceCollection services)
        {
            return services.AddSingleton<ITelegramBotClient>(
                ctx => new TelegramBotClient(ctx.GetService<AppConfig>().ApiKey));
        }

        internal static IServiceCollection AddSerilogLogging(this IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .CreateLogger();

            services.AddSingleton<ILogger>(Log.Logger);
            return services;
        }

        internal static IServiceCollection AddAhRulesFile(this IServiceCollection services)
        {
            var rules = new List<RuleItem>();
            var parsed = JsonSerializer.Deserialize<List<Rule>>(File.ReadAllText("rules.json"), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            rules.AddRange(parsed);

            var subrules = parsed.Where(x => x.Rules != null).SelectMany(x => x.Rules).ToList();
            rules.AddRange(subrules);

            var subsubrules = subrules.Where(x => x.Rules != null).SelectMany(x => x.Rules).ToList();
            rules.AddRange(subsubrules);

            return services.AddSingleton<List<RuleItem>>(
                ctx => rules);
        }

        internal static IServiceCollection AddMessageHandlers(this IServiceCollection services)
        {
            services.AddSingleton<RulesMessageHandler>(
                x => new RulesMessageHandler(x.GetRequiredService<ILogger>(), x.GetRequiredService<List<RuleItem>>(), null));
            services.AddSingleton<CommandMessageHandler>(
                x => new CommandMessageHandler(x.GetRequiredService<RulesMessageHandler>()));
            services.AddSingleton<UnknownMessageHandler>(
                x => new UnknownMessageHandler(x.GetRequiredService<CommandMessageHandler>()));
            services.AddSingleton<IMessageHandler, LongMessageHandler>(
                x => new LongMessageHandler(x.GetRequiredService<UnknownMessageHandler>()));
            return services;
        }
    }
}
