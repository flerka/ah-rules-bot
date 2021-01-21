using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AhRulesBot
{
    public class Worker : BackgroundService
    {
        private readonly AppConfig _config;
        private readonly ITelegramBotClient _botClient;
        private readonly List<RuleItem> _rules;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger _logger;

        private static User _botAsUser;

        public Worker(
            ILogger logger,
            AppConfig config,
            ITelegramBotClient botClient,
            IHostApplicationLifetime hostApplicationLifetime,
            List<RuleItem> rules)
        {
            _logger = logger;
            _config = config;
            _hostApplicationLifetime = hostApplicationLifetime;
            _botClient = botClient;
            _rules = rules;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _botAsUser = await _botClient.GetMeAsync();
                _logger.Information($"Bot's name: {_botAsUser.FirstName}");

                _botClient.OnMessage += OnMessage;
                _botClient.StartReceiving(new UpdateType[] { UpdateType.Message });

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "{nameof(Worker)}.{nameof(ExecuteAsync)} Some unhandled error occurred, stopping application.");
                _hostApplicationLifetime.StopApplication();
            }
        }

        private async void OnMessage(object? sender, MessageEventArgs e)
        {
            Message msg = e.Message;
            Chat chat = msg.Chat;

            if (string.IsNullOrEmpty(msg?.Text))
                return;

            _logger.Information($"@{msg.From.Username}: {e.Message.Text}");

            if (chat.Id != _config.AHChatId && chat.Id != _config.TestChatId)
            {
                await _botClient.SendTextMessageAsync(chat.Id, "I don't know you.");
                return;
            }

            if (DateTime.UtcNow.Subtract(msg.Date).TotalMinutes > 4)
            {
                return;
            }

            OnMessageToChat(chat.Id, msg.Text);
        }

        private async void OnMessageToChat(long chatId, string msg)
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
                        await SenBatchMessagesToChat(chatId, rules.ToList());
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

        private Task<Message> SendMessageToChat(long chatId, string? msg)
        {
            if (string.IsNullOrEmpty(msg))
                return Task.FromResult(new Message());
            return _botClient.SendTextMessageAsync(new ChatId(chatId), msg, ParseMode.Html);
        }

        private async Task SenBatchMessagesToChat(long chatId, List<string> msgs)
        {
            if (msgs == null || msgs.Count == 0)
                return;

            foreach (var msg in msgs)
                await _botClient.SendTextMessageAsync(new ChatId(chatId), msg, ParseMode.Html);
        }
    }
}
