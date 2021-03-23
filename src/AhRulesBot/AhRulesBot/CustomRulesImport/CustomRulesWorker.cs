using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using AhRulesBot.Models;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using System.Collections.Generic;
using Google.Apis.Services;
using Google.Apis.Drive.v3;
using AhRulesBot.Infrastructure;
using System.Linq;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using CsvHelper;
using System.Threading.Channels;

namespace AhRulesBot.OldMessagesProcessing
{
    internal class CustomRulesWorker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ChannelWriter<List<CustomRuleItem>> _channel;
        private readonly AppConfig _config;

        public CustomRulesWorker(
            ILogger logger,
            AppConfig config,
            ChannelWriter<List<CustomRuleItem>> channel)
        {
            _channel = channel;
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var drive = GetDrive(GetCredentialParameters());
                    var customRules = await GetCutomRulesFromDrive(drive, cancellationToken);
                    await _channel.WriteAsync(customRules, cancellationToken);

                    await Task.Delay(TimeSpan.FromHours(5));
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "failed {name}", nameof(CustomRulesWorker));
                }
            }

            _logger.Information("stopping {name}", nameof(CustomRulesWorker));
        }

        private async Task<List<CustomRuleItem>> GetCutomRulesFromDrive(DriveService drive, CancellationToken cancellationToken)
        {
            var googleFileStream = await drive.Files.Export(_config.GoogleFileId, "text/csv").ExecuteAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(googleFileStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<CustomRuleItem>().ToList();
        }

        private DriveService GetDrive(JsonCredentialParameters credParams)
        {
            var accountCred = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(credParams.ClientEmail)
               {
                   Scopes = new string[] { DocsService.Scope.DriveReadonly }
               }
               .FromPrivateKey(credParams.PrivateKey));

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = accountCred,
                ApplicationName = "custom-ah-rules",
            });
        }

        private JsonCredentialParameters GetCredentialParameters()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy { ProcessDictionaryKeys = true }
                },
                Formatting = Formatting.Indented
            };
            return JsonConvert.DeserializeObject<JsonCredentialParameters>(File.ReadAllText(_config.GoogleCredFilePath), settings);
        }
    }
}
