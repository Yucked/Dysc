using System.Text.Json.Serialization;

namespace Dysc.Providers.SoundCloud.Entities {
    internal struct SoundCloudFormat {
        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("mime_type")]
        public string MimeType { get; set; }
    }
}