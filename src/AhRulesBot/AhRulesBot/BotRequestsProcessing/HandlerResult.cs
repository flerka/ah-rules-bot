﻿using System;
using System.Collections.Generic;

namespace AhRulesBot.BotRequestsProcessing
{
    public class HandlerResult
    {
        public List<string> Data { get; set; } = new List<string>();
        public TimeSpan? Ttl { get; set; }
    }
}
