using System;
using Newtonsoft.Json;

namespace RPBot
{
    internal sealed class Config
    {
        [JsonProperty("token")]
        public string Token { get; private set; } = string.Empty;

        [JsonProperty("command_prefix")]
        public string CommandPrefix { get; private set; } = "!";

        [JsonProperty("shards")]
        public int ShardCount { get; private set; } = 1;
    }
}
