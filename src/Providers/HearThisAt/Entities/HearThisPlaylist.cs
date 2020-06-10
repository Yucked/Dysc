using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dysc.Interfaces;

namespace Dysc.Providers.HearThisAt.Entities {
	// Requires a separate call to API
	internal class HearThisPlaylist : IPlaylistResult {
		ISourceAuthor IPlaylistResult.Author
			=> HearThisAuthor;

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("permalink_url")]
		public string Url { get; set; }

		[JsonPropertyName("artwork_url")]
		public string ArtworkUrl { get; set; }

		[JsonIgnore]
		public long Duration { get; internal set; }

		[JsonPropertyName("user")]
		public HearThisAuthor HearThisAuthor { get; set; }

		[JsonIgnore]
		public IReadOnlyList<ITrackResult> Tracks { get; internal set; }
	}
}