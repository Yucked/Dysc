using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dysc.Providers.YouTube.API {
    public struct PlayerResponse {
        [JsonPropertyName("playabilityStatus")]
        public PlayabilityStatus Playability { get; set; }

        [JsonPropertyName("streamingData")]
        public StreamingData Streaming { get; set; }
    }

    public struct PlayabilityStatus {
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public struct StreamingData {
        [JsonPropertyName("adaptiveFormats")]
        public IReadOnlyList<AdaptiveFormat> AdaptiveFormats { get; set; }
    }
}