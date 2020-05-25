using System.Text.Json.Serialization;

namespace Dysc.Providers.BandCamp.Entities {
    internal struct BandCampFile {
        [JsonPropertyName("mp3-128")]
        public string Mp3Url { get; set; }
    }
}