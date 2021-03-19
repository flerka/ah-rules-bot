using AhRulesBot.MessageProcessing.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AhRulesBot.MessageProcessing
{
    public class RulesMessageHandler : IMessageHandler
    {
        private IMessageHandler _next;
        private readonly List<RuleItem> _rules;
        private readonly ILogger _logger;

        public RulesMessageHandler(
            ILogger logger,
            List<RuleItem> rules,
            IMessageHandler next)
        {
            _logger = logger;
            _rules = rules;
            _next = next;
        }

        public List<string> Handle(string message)
        {
            var command = TryParseAsRulesRequest(message);
            if (command)
            {
                return ProcessRulesRequest(message);
            }

            return _next != null ? _next.Handle(message) : new List<string>();
        }

        private List<string> ProcessRulesRequest(string message)
        {
            Func<RuleItem, bool> messageContains = item => item.Title.Contains(message, StringComparison.InvariantCultureIgnoreCase)
                    || item.Id.Contains(message, StringComparison.InvariantCultureIgnoreCase);
            Func<RuleItem, bool> messageExact = item => item.Title.Equals(message, StringComparison.InvariantCultureIgnoreCase)
                || item.Id.Equals(message, StringComparison.InvariantCultureIgnoreCase);

            return _rules.Where(messageContains)
                .OrderByDescending(messageExact).ThenBy(i => CalcLevenshteinDistance(message, i.Title))
                .Select(i => $"<b>{i.Title}</b>\n{i.Text}").ToList();
        }

        private bool TryParseAsRulesRequest(string message)
        {
            return true;
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

    }
}
