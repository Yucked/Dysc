using System.Text.Json.Serialization;

namespace Dysc.Providers.SoundCloud.Entities {
	internal struct SoundCloudStream {
		[JsonPropertyName("data")]
		public SoundCloudData[] Data { get; set; }
	}

	internal struct SoundCloudData {
		[JsonPropertyName("media")]
		public SoundCloudMedia Media { get; set; }
	}

	internal struct SoundCloudMedia {
		[JsonPropertyName("transcodings")]
		public SoundCloudTranscoding[] Transcodings { get; set; }
	}
}