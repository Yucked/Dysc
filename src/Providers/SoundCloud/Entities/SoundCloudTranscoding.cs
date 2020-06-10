using System.Text.Json.Serialization;

namespace Dysc.Providers.SoundCloud.Entities {
	internal class SoundCloudTranscoding {
		[JsonPropertyName("url")]
		public string Url { get; set; }

		[JsonPropertyName("preset")]
		public string Preset { get; set; }
	}
}