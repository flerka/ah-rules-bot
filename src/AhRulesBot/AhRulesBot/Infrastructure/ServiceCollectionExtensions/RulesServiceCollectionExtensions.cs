using AhRulesBot.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AhRulesBot.Infrastructure.ServiceCollectionExtensions
{
    internal static class RulesServiceCollectionExtensions
    {
        internal static IServiceCollection AddAhRulesFile(this IServiceCollection services)
        {
            return services.AddSingleton(x => GetRulesFromFile(x.GetRequiredService<AppConfig>().RulesFilePath));
        }

        private static List<RuleItem> GetRulesFromFile(string location)
        {
            var rules = new List<RuleItem>();
            var settings = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var parsed = JsonSerializer.Deserialize<List<Rule>>(File.ReadAllText(location), settings) ?? new();

            rules.AddRange(parsed);

            var subrules = parsed.Where(x => x.Rules != null).SelectMany(x => x.Rules).ToList();
            rules.AddRange(subrules);

            var subsubrules = subrules.Where(x => x.Rules != null).SelectMany(x => x.Rules).ToList();
            rules.AddRange(subsubrules);

            return rules;
        }
    }
}
