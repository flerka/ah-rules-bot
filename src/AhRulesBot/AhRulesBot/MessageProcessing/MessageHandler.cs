using AhRulesBot.Infrastructure;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AhRulesBot.MessageProcessing
{
    public class MessageHandler : IMessageHandler
    {
        private readonly AppConfig _config;
        private readonly ITelegramBotClient _botClient;
        private readonly List<RuleItem> _rules;
        private readonly ILogger _logger;

        public MessageHandler(
            ILogger logger,
            AppConfig config,
            ITelegramBotClient botClient,
            List<RuleItem> rules)
        {
            _logger = logger;
            _config = config;
            _botClient = botClient;
            _rules = rules;
        }

        public async Task Handle(Message message)
        {
            Chat chat = message.Chat;

            if (string.IsNullOrEmpty(message?.Text))
                return;

            _logger.Information($"@{message.From.Username}: {message.Text}");

            if (chat.Id != _config.AHChatId && chat.Id != _config.TestChatId)
            {
                await _botClient.SendTextMessageAsync(chat.Id, "I don't know you.");
                return;
            }

            if (DateTime.UtcNow.Subtract(message.Date).TotalMinutes > 4)
            {
                return;
            }

            await OnMessageToChat(chat.Id, message.Text);
        }

        private async Task OnMessageToChat(long chatId, string msg)
        {
            if (msg.StartsWith('/') || msg.StartsWith("!"))
                msg = msg.Remove(0, 1);
            if (msg.EndsWith(_config.BotName))
                msg = msg.Remove(msg.LastIndexOf(_config.BotName));

            try
            {
                Func<RuleItem, bool> checkMessage = item => item.Title.Contains(msg, StringComparison.InvariantCultureIgnoreCase)
                    || item.Id.Contains(msg, StringComparison.InvariantCultureIgnoreCase);
                var rules = _rules.Where(checkMessage).Select(i => $"<b>{i.Title}</b>\n{i.Text}");

                if (rules.Any())
                {
                    var message = string.Join("\n\n\n\n", rules);
                    if (message.Length > 4000)
                    {
                        await SendBatchMessagesToChat(chatId, rules.ToList());
                    }
                    else
                    {
                        await SendMessageToChat(chatId, message);
                    }
                }
                else
                    await SendMessageToChat(chatId, "I didn't find any information");
            }
            catch
            {
                await SendMessageToChat(chatId, "Something went wrong");
            }
        }

        private async Task SendMessageToChat(long chatId, string msg)
        {
            await _botClient.SendTextMessageAsync(new ChatId(chatId), msg, ParseMode.Html);
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
