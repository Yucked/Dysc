using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dysc.Providers.YouTube.Entities {
    internal struct YouTubeSearch {
        [JsonPropertyName("video")]
        public IEnumerable<YouTubeVideo> Video { get; set; }
    }
}