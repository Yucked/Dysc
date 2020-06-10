using System.Text.Json.Serialization;
using Dysc.Interfaces;

namespace Dysc.Providers.HearThisAt.Entities {
	internal sealed class HearThisTrack : ITrackResult {
		[JsonIgnore]
		long ITrackResult.Duration
			=> long.Parse(RawDuration);

		[JsonIgnore]
		ISourceAuthor ITrackResult.Author
			=> User;

		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("duration")]
		public string RawDuration { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("permalink_url")]
		public string Url { get; set; }

		[JsonPropertyName("artwork_url")]
		public string ArtworkUrl { get; set; }

		[JsonPropertyName("user")]
		public HearThisAuthor User { get; set; }

		[JsonPropertyName("stream_url")]
		public string StreamUrl { get; set; }

		[JsonIgnore]
		public bool IsStreamable
			=> !string.IsNullOrWhiteSpace(StreamUrl);
	}
}