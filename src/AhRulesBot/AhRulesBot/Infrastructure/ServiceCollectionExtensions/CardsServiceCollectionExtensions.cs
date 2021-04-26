using AhRulesBot.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AhRulesBot.Infrastructure.ServiceCollectionExtensions
{
    internal static class CardsServiceCollectionExtensions
    {
        internal static IServiceCollection AddCardsFile(this IServiceCollection services)
        {
            return services.AddSingleton(x =>
            {
                var data = JsonSerializer.Deserialize<List<Card>>(File.ReadAllText(x.GetRequiredService<AppConfig>().CardsFilePath));
                if (data == null)
                {
                    throw new Exception("cards is null");
                }
                return data;
            });
        }
    }
}
