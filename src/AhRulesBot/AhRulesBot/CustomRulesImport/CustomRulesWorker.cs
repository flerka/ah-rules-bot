using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using AhRulesBot.Models;
using System.IO;
using System.Collections.Generic;
using AhRulesBot.Infrastructure;
using System.Linq;
using System.Globalization;
using CsvHelper;
using System.Threading.Channels;
using AhRulesBot.BotRequestsProcessing.Interfaces;

namespace AhRulesBot.CustomRulesImport
{
    internal class CustomRulesWorker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IDriveService _rulesDriveService;
        private readonly ChannelWriter<List<CustomRuleItem>> _channel;
        private readonly AppConfig _config;

        public CustomRulesWorker(
            ILogger logger,
            AppConfig config,
            IDriveService rulesDriveService,
            ChannelWriter<List<CustomRuleItem>> channel)
        {
            _channel = channel;
            _logger = logger;
            _rulesDriveService = rulesDriveService;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var customRules = await GetCutomRulesFromDrive(cancellationToken);
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

        private async Task<List<CustomRuleItem>> GetCutomRulesFromDrive(CancellationToken cancellationToken)
        {
            var googleFileStream = await _rulesDriveService.Files.Export(_config.GoogleFileId, "text/csv")
                .ExecuteAsStreamAsync(cancellationToken);

            using var reader = new StreamReader(googleFileStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<CustomRuleItem>().ToList();
        }
    }
}
