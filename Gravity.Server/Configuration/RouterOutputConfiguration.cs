﻿using Gravity.Server.Pipeline;
using Newtonsoft.Json;

namespace Gravity.Server.Configuration
{
    internal class RouterOutputConfiguration: RouterGroupConfiguration
    {
        [JsonProperty("to")]
        public string RouteTo { get; set; }

        [JsonIgnore]
        public NodeOutput ProcessingNode { get; set; }

        public void Sanitize()
        { }
    }
}