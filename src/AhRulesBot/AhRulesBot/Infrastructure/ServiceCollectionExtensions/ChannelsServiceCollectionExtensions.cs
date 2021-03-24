using AhRulesBot.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Channels;

namespace AhRulesBot.Infrastructure.ServiceCollectionExtensions
{
    internal static class ChannelsServiceCollectionExtensions
    {

        internal static IServiceCollection AddMsgToRemoveChannel(this IServiceCollection services)
        {
            services.AddSingleton<Channel<TelegramMessageInfo>>(Channel.CreateUnbounded<TelegramMessageInfo>(new UnboundedChannelOptions()
            {
                SingleReader = true,
                SingleWriter = true,
            }));
            services.AddSingleton<ChannelReader<TelegramMessageInfo>>(svc => svc.GetRequiredService<Channel<TelegramMessageInfo>>().Reader);
            services.AddSingleton<ChannelWriter<TelegramMessageInfo>>(svc => svc.GetRequiredService<Channel<TelegramMessageInfo>>().Writer);
            return services;
        }

        internal static IServiceCollection AddCustomRulesChannel(this IServiceCollection services)
        {
            services.AddSingleton<Channel<List<CustomRuleItem>>>(Channel.CreateUnbounded<List<CustomRuleItem>>(new UnboundedChannelOptions()
            {
                SingleReader = true,
                SingleWriter = true,
            }));
            services.AddSingleton<ChannelReader<List<CustomRuleItem>>>(svc => svc.GetRequiredService<Channel<List<CustomRuleItem>>>().Reader);
            services.AddSingleton<ChannelWriter<List<CustomRuleItem>>>(svc => svc.GetRequiredService<Channel<List<CustomRuleItem>>>().Writer);
            return services;
        }
    }
}
