﻿using AhRulesBot.BotRequestsProcessing.Interfaces;
using AhRulesBot.Infrastructure;
using AhRulesBot.Models;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AhRulesBot.BotRequestsProcessing
{
    internal class MessageValidator : IMessageValidator
    {
        private readonly AppConfig _config;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger _logger;

        private readonly ConcurrentDictionary<int, bool> _usersCache = new ConcurrentDictionary<int, bool>();
        private readonly MemoryCache _requestsCache = new MemoryCache(new MemoryCacheOptions());
        private readonly MemoryCache _bannedCache = new MemoryCache(new MemoryCacheOptions());
        private readonly TimeSpan _requestCacheExpiration = TimeSpan.FromMinutes(2);
        private readonly TimeSpan _bannedCacheTimeout = TimeSpan.FromMinutes(5);

        private const int MaxMessagesInMinute = 5;

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

            if (IsSpamming(message))
            {
                return false;
            }

            return true;
        }

        private bool IsSpamming(Message message)
        {
            var bannedCacheKey = $"{message.From.Id}";
            var requestCacheKey = $"{message.From.Id}_{DateTime.UtcNow.ToString("H: mm", CultureInfo.InvariantCulture)}";

            if (_bannedCache.TryGetValue(bannedCacheKey, out bool isBanned) && isBanned)
            {
                return true;
            }

            _requestsCache.TryGetValue(requestCacheKey, out int value);
            value++;
            _requestsCache.Set(requestCacheKey, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_requestCacheExpiration));

            if (value > MaxMessagesInMinute)
            {
                _bannedCache.Set(bannedCacheKey, true, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_bannedCacheTimeout));
                return true;
            }

            return false;
        }

        private async Task<bool> IsValidSender(Message message)
        {
            if (message.Chat.Type != ChatType.Private)
            {
                return message.Chat.Id == _config.AHChatId || message.Chat.Id == _config.TestChatId;
            }

            var isAllowed = false;
            if (!_usersCache.TryGetValue(message.From.Id, out isAllowed))
            {
                return isAllowed;
            }

            try
            {
                var member = await _botClient.GetChatMemberAsync(_config.TestChatId, message.From.Id);
                isAllowed = member != null && member.Status != ChatMemberStatus.Left;
                _usersCache.TryAdd(message.From.Id, isAllowed);
            }
            catch
            {
                _logger.Warning($"Can't verify if user is member of the group. {_config.TestChatId}, {message.From.Id}");
            }

            return isAllowed;
        }
    }
}
