using System.Text.Json.Serialization;

namespace Dysc.Providers.SoundCloud.Entities {
	internal class SoundCloudMedia {
		[JsonPropertyName("transcodings")]
		public SoundCloudTranscoding[] Transcodings { get; set; }
	}
}