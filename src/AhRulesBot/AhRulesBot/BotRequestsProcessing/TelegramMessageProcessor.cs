using AhRulesBot.BotRequestsProcessing.Interfaces;
using AhRulesBot.Infrastructure;
using AhRulesBot.MessageProcessing.Interfaces;
using AhRulesBot.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AhRulesBot.BotRequestsProcessing
{
    internal class TelegramMessageProcessor : ITelegramMessageProcessor
    {
        private readonly IMessageValidator _messageValidator;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;
        private readonly IMessageHandler _textMessageHandler;
        private readonly AppConfig _config;
        private readonly ChannelWriter<TelegramMessageInfo> _channel;

        public TelegramMessageProcessor(
            IMessageValidator messageValidator,
            ITelegramBotClient botClient, ILogger logger,
            IMessageHandler textMessageHandler,
            AppConfig config,
            ChannelWriter<TelegramMessageInfo> channel)
        {
            _messageValidator = messageValidator;
            _botClient = botClient;
            _logger = logger;
            _textMessageHandler = textMessageHandler;
            _config = config;
            _channel = channel;
        }

        public async Task Process(Message msg, CancellationToken cancellationToken)
        {
            if (!await _messageValidator.IsValid(msg))
            {
                return;
            }

            _logger.Information("{Username} {Id}: {Text}", msg.From.Username, msg.From.Id, msg.Text);

            try
            {
                // parse command symbol
                var text = msg.Text.Remove(0, 1);
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }

                if (text.EndsWith(_config.BotName))
                    text = text.Remove(text.LastIndexOf(_config.BotName));

                var result = _textMessageHandler.Handle(text);
                await SendBatchMessagesToChat(msg.Chat.Id, result.Data, result.Ttl, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.Error(
                    e,
                    "Something went wrong. {Username} {Id}: {Text}",
                    msg.From.Username, msg.From.Id, msg.Text);
            }
        }

        private async Task SendBatchMessagesToChat(long chatId, List<string> msgs, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
        {
            if (msgs == null || msgs.Count == 0)
                return;

            foreach (var msg in msgs)
            {
                var result = await _botClient.SendTextMessageAsync(new ChatId(chatId), msg, ParseMode.Html, cancellationToken: cancellationToken);
                if (ttl.HasValue)
                {
                    await _channel.WriteAsync(new TelegramMessageInfo
                    { ChatId = chatId, Id = result.MessageId, Ttl = ttl.Value, SentUtc = DateTime.UtcNow }, cancellationToken);
                }
            }

        }
    }
}
