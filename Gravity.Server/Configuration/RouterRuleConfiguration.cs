﻿using Newtonsoft.Json;

namespace Gravity.Server.Configuration
{
    internal class RouterRuleConfiguration
    {
        [JsonProperty("disabled")]
        public bool Disabled { get; set; }

        [JsonProperty("condition")]
        public string Condition { get; set; }

        [JsonProperty("negate")]
        public bool Negate { get; set; }
    }
}