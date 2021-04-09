using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Threading.Channels;
using AhRulesBot.Models;
using System.Collections.Concurrent;

namespace AhRulesBot.OldMessagesProcessing
{
    internal class OldMessagesWorker : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger _logger;
        private readonly ChannelReader<TelegramMessageInfo> _channel;
        private readonly ConcurrentQueue<TelegramMessageInfo> _messagesToDelete = new();

        public OldMessagesWorker(
            ILogger logger,
            ITelegramBotClient botClient,
            IHostApplicationLifetime hostApplicationLifetime,
            ChannelReader<TelegramMessageInfo> channel)
        {
            _logger = logger;
            _channel = channel;
            _hostApplicationLifetime = hostApplicationLifetime;
            _botClient = botClient;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var msg = await _channel.ReadAsync(cancellationToken);
                    var delayDuration = msg.SentUtc + msg.Ttl - DateTime.UtcNow;
                    if ((int)delayDuration.TotalSeconds > 0)
                    {
                        await Task.Delay(delayDuration, cancellationToken);
                    }
                    await _botClient.DeleteMessageAsync(msg.ChatId, msg.Id, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "failed {name}", nameof(OldMessagesWorker));
                }
            }

            _logger.Information("stopping {name}", nameof(OldMessagesWorker));
        }
    }
}
