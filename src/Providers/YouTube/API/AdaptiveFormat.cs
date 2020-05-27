using System.Text.Json.Serialization;

namespace Dysc.Providers.YouTube.API {
	public struct AdaptiveFormat {
		[JsonPropertyName("url")]
		public string Url { get; set; }

		[JsonPropertyName("mimeType")]
		public string MimeType { get; set; }

		[JsonPropertyName("bitrate")]
		public long Bitrate { get; set; }

		[JsonPropertyName("contentLength")]
		public string ContentLength { get; set; }

		[JsonPropertyName("audioSampleRate")]
		public string AudioSampleRate { get; set; }

		[JsonPropertyName("audioChannels")]
		public long AudioChannels { get; set; }
	}
}