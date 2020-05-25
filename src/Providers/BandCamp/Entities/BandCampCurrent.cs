using System.Text.Json.Serialization;

namespace Dysc.Providers.BandCamp.Entities {
    internal struct BandCampCurrent {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }
    }
}