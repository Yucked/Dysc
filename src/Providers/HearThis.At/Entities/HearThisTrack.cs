using System.Text.Json.Serialization;
using Dysc.Infos;

namespace Dysc.Providers.HearThis.At.Entities {
	internal struct HearThisTrack {
		[JsonPropertyName("duration")]
		public string Duration { get; set; }

		[JsonPropertyName("permalink")]
		public string Permalink { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("permalink_url")]
		public string PermalinkUrl { get; set; }

		[JsonPropertyName("artwork_url")]
		public string ArtworkUrl { get; set; }

		[JsonPropertyName("user")]
		public HearThisAuthor Author { get; set; }

		[JsonPropertyName("stream_url")]
		public string StreamUrl { get; set; }

		public TrackInfo ToTrackInfo
			=> new TrackInfo {
				Id = Permalink,
				Author = Author.ToAuthorInfo,
				Title = Title,
				Provider = ProviderType.HearThisAt,
				Duration = int.Parse(Duration) * 1000,
				Url = PermalinkUrl,
				ArtworkUrl = ArtworkUrl,
				CanStream = !string.IsNullOrWhiteSpace(StreamUrl)
			};
	}
}