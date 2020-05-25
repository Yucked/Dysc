using System.Text.Json.Serialization;

namespace Dysc.Providers.YouTube.Entities {
    internal class YouTubeResult {
        [JsonPropertyName("encrypted_id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }
    }
}