using AhRulesBot.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AhRulesBot.Infrastructure.ServiceCollectionExtensions
{
    internal static class CardsServiceCollectionExtensions
    {
        internal static IServiceCollection AddCardsFile(this IServiceCollection services)
        {
            return services.AddSingleton(x => JsonSerializer.Deserialize<List<Card>>(File.ReadAllText(x.GetService<AppConfig>().CardsFilePath)));
        }
    }
}
