using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Extensions.Polling;
using AhRulesBot.MessageProcessing;

namespace AhRulesBot
{
    public class Worker : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger _logger;
        private readonly ITelegramMessageProcessor _telegramMessageProcessor;

        public Worker(
            ILogger logger,
            ITelegramBotClient botClient,
            IHostApplicationLifetime hostApplicationLifetime,
            ITelegramMessageProcessor telegramMessageProcessor)
        {
            _logger = logger;
            _telegramMessageProcessor = telegramMessageProcessor;
            _hostApplicationLifetime = hostApplicationLifetime;
            _botClient = botClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var botName = await _botClient.GetMeAsync();
                _logger.Information($"Bot's name: {botName.FirstName}");

                var updateReceiver = new QueuedUpdateReceiver(_botClient);
                updateReceiver.StartReceiving(new UpdateType[] { UpdateType.Message });

                await foreach (Update update in updateReceiver.YieldUpdatesAsync())
                {
                    await _telegramMessageProcessor.Process(update.Message);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "{nameof(Worker)}.{nameof(ExecuteAsync)} Some unhandled error occurred, stopping application.");
                _hostApplicationLifetime.StopApplication();
            }
        }
    }
}
