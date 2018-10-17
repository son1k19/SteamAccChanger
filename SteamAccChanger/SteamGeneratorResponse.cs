using Newtonsoft.Json;

namespace SteamAccChanger
{
    internal class SteamGeneratorResponse
    {
        [JsonProperty("success")]
        public int Success { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("email")]
        public string EMail { get; set; }
    }
}
