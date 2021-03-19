using AhRulesBot.Infrastructure;
using AhRulesBot.MessageProcessing.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AhRulesBot.MessageProcessing
{
    internal class TelegramMessageProcessor : ITelegramMessageProcessor
    {
        private readonly IMessageValidator _messageValidator;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;
        private readonly IMessageHandler _textMessageHandler;
        private readonly AppConfig _config;

        public TelegramMessageProcessor(
            IMessageValidator messageValidator,
            ITelegramBotClient botClient, ILogger logger,
            IMessageHandler textMessageHandler,
            AppConfig config)
        {
            _messageValidator = messageValidator;
            _botClient = botClient;
            _logger = logger;
            _textMessageHandler = textMessageHandler;
            _config = config;
        }

        public async Task Process(Message msg)
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

                if (text.EndsWith(_config.BotName))
                    text = text.Remove(text.LastIndexOf(_config.BotName));

                var result = _textMessageHandler.Handle(text);
                await SendBatchMessagesToChat(msg.Chat.Id, result);
            }
            catch (Exception e)
            {
                _logger.Error(
                    e,
                    "Something went wrong. {Username} {Id}: {Text}",
                    msg.From.Username, msg.From.Id, msg.Text);
            }
        }

        private async Task SendBatchMessagesToChat(long chatId, List<string> msgs)
        {
            if (msgs == null || msgs.Count == 0)
                return;

            foreach (var msg in msgs)
                await _botClient.SendTextMessageAsync(new ChatId(chatId), msg, ParseMode.Html);
        }
    }
}
