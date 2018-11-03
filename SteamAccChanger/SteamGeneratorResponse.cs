using Newtonsoft.Json;

namespace SteamAccChanger
{
    internal class SteamGeneratorResponse
    {
        [JsonProperty("success")]
        public int Success { get; set; } = -1; //default value set to -1 for proper success checking after JSON deserialization

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("email")]
        public string EMail { get; set; }

        [JsonProperty("error")]
        public string ErrorMessage { get; set; }
    }
}
