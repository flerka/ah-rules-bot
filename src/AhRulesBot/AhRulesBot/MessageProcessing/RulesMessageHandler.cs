using AhRulesBot.Infrastructure;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AhRulesBot.MessageProcessing
{
    public class RulesMessageHandler : IMessageHandler
    {
        private readonly AppConfig _config;
        private readonly ITelegramBotClient _botClient;
        private readonly List<RuleItem> _rules;
        private readonly ILogger _logger;

        private static readonly ConcurrentDictionary<int, bool> usersCache = new ConcurrentDictionary<int, bool>();

        public RulesMessageHandler(
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
            if (string.IsNullOrEmpty(message?.Text))
                return;

            // Only commands should be processed
            if (!message.Text.StartsWith('/'))
            {
                return;
            }

            _logger.Information($"@{message.From.Username} {message.From.Id}: {message.Text}");

            if (! await IsValidSender(message))
            {
                return; 
            }

            if (DateTime.UtcNow.Subtract(message.Date).TotalMinutes > 4)
            {
                return;
            }

            await OnMessageToChat(message.Chat.Id, message.Text);
        }

        private async Task OnMessageToChat(long chatId, string msg)
        {
            // parse command symbol
            msg = msg.Remove(0, 1);

            if (msg.EndsWith(_config.BotName))
                msg = msg.Remove(msg.LastIndexOf(_config.BotName));
            try
            {
                Func<RuleItem, bool> messageContains = item => item.Title.Contains(msg, StringComparison.InvariantCultureIgnoreCase)
                    || item.Id.Contains(msg, StringComparison.InvariantCultureIgnoreCase);
                Func<RuleItem, bool> messageExact = item => item.Title.Equals(msg, StringComparison.InvariantCultureIgnoreCase)
                    || item.Id.Equals(msg, StringComparison.InvariantCultureIgnoreCase);

                var rules = _rules.Where(messageContains)
                    .OrderByDescending(messageExact).ThenBy(i => CalcLevenshteinDistance(msg, i.Title))
                    .Select(i => $"<b>{i.Title}</b>\n{i.Text}");

                if (rules.Any())
                {
                    var message = string.Join("\n\n\n\n", rules);
                    if (message.Length <= 4000)
                    {
                        await SendMessageToChat(chatId, message);
                    }
                    else
                    {
                        await SendBatchMessagesToChat(chatId, rules.Take(2).ToList());
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

        private async Task SendMessageToChat(long chatId, string msg)
        {
            await _botClient.SendTextMessageAsync(new ChatId(chatId), msg, ParseMode.Html);
        }

        private int CalcLevenshteinDistance(string source1, string source2) 
        {
            var source1Length = source1.Length;
            var source2Length = source2.Length;

            var matrix = new int[source1Length + 1, source2Length + 1];

            // First calculation, if one entry is empty return full length
            if (source1Length == 0)
                return source2Length;

            if (source2Length == 0)
                return source1Length;

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
            for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }

            // Calculate rows and collumns distances
            for (var i = 1; i <= source1Length; i++)
            {
                for (var j = 1; j <= source2Length; j++)
                {
                    var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }
            // return result
            return matrix[source1Length, source2Length];
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
