using AhRulesBot.BotRequestsProcessing.Handlers;
using AhRulesBot.MessageProcessing.Interfaces;
using AhRulesBot.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Collections.Generic;
using System.Threading.Channels;

namespace AhRulesBot.Infrastructure.ServiceCollectionExtensions
{
    internal static class HandlersServiceCollectionExtensions
    {
        internal static IServiceCollection AddMessageHandlers(this IServiceCollection services)
        {
            services.AddSingleton<CustomRulesMessageHandler>(
                x => new CustomRulesMessageHandler(x.GetRequiredService<ILogger>(), x.GetRequiredService<ChannelReader<List<CustomRuleItem>>>(), null));
            services.AddSingleton<CardsMessageHandler>(
                x => new CardsMessageHandler(x.GetRequiredService<ILogger>(), x.GetRequiredService<List<Card>>(), x.GetRequiredService<CustomRulesMessageHandler>()));
            services.AddSingleton<RulesMessageHandler>(
                x => new RulesMessageHandler(x.GetRequiredService<ILogger>(), x.GetRequiredService<List<RuleItem>>(), x.GetRequiredService<CardsMessageHandler>()));
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
