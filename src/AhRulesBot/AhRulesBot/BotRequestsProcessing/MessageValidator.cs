using AhRulesBot.BotRequestsProcessing.Interfaces;
using AhRulesBot.Infrastructure;
using AhRulesBot.Models;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AhRulesBot.BotRequestsProcessing
{
    internal class MessageValidator : IMessageValidator
    {
        private static readonly ConcurrentDictionary<int, bool> usersCache = new ConcurrentDictionary<int, bool>();

        private readonly AppConfig _config;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        public MessageValidator(
            ILogger logger,
            AppConfig config,
            ITelegramBotClient botClient,
            List<RuleItem> rules)
        {
            _logger = logger;
            _config = config;
            _botClient = botClient;
        }

        public async Task<bool> IsValid(Message message)
        {
            if (string.IsNullOrEmpty(message?.Text))
                return false;

            // Only commands should be processed
            if (!message.Text.StartsWith('/'))
                return false;

            if (!await IsValidSender(message))
                return false;

            if (DateTime.UtcNow.Subtract(message.Date).TotalMinutes > 4)
                return false;

            return true;
        }

        private async Task<bool> IsValidSender(Message message)
        {
            if (message.Chat.Type == ChatType.Private)
            {
                var isAllowed = false;
                if (!usersCache.TryGetValue(message.From.Id, out isAllowed))
                {
                    try
                    {
                        var member = await _botClient.GetChatMemberAsync(_config.TestChatId, message.From.Id);
                        isAllowed = member != null && member.Status != ChatMemberStatus.Left;
                        usersCache.TryAdd(message.From.Id, isAllowed);
                    }
                    catch
                    {
                        _logger.Warning($"Can't verify if user is member of the group. {_config.TestChatId}, {message.From.Id}");
                    }
                }


                return isAllowed;
            }

            return message.Chat.Id == _config.AHChatId || message.Chat.Id == _config.TestChatId;
        }
    }
}
