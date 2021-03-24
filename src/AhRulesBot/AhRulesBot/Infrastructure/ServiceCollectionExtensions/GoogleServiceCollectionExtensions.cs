using AhRulesBot.CustomRulesImport;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace AhRulesBot.Infrastructure.ServiceCollectionExtensions
{
    internal static class GoogleServiceCollectionExtensions
    {
        public static IServiceCollection AddRulesDriveService(this IServiceCollection services)
        {
            return services.AddSingleton(x => GetDrive(x.GetService<AppConfig>().GoogleCredFilePath));
        }

        private static RulesDriveService GetDrive(string googleCredPath)
        {
            var credParams = GetCredentialParameters(googleCredPath);
            var accountCred = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(credParams.ClientEmail)
               {
                   Scopes = new string[] { DocsService.Scope.DriveReadonly }
               }
               .FromPrivateKey(credParams.PrivateKey));

            return new RulesDriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = accountCred,
                ApplicationName = "custom-ah-rules",
            });
        }

        private static JsonCredentialParameters GetCredentialParameters(string googleCredPath)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy { ProcessDictionaryKeys = true }
                },
                Formatting = Formatting.Indented
            };

            return JsonConvert.DeserializeObject<JsonCredentialParameters>(File.ReadAllText(googleCredPath), settings);
        }
    }
}
