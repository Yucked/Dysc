using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dysc.Interfaces;

namespace Dysc.Providers.SoundCloud.Entities {
	internal class SoundCloudPlaylist : IPlaylistResult {
		string IPlaylistResult.Id
			=> $"{RawId}";

		ISourceAuthor IPlaylistResult.Author
			=> User;

		IReadOnlyList<ITrackResult> IPlaylistResult.Tracks
			=> CloudTracks;

		[JsonPropertyName("duration")]
		public long Duration { get; set; }

		[JsonPropertyName("permalink_url")]
		public string Url { get; set; }

		[JsonPropertyName("tracks")]
		public IReadOnlyList<SoundCloudTrack> CloudTracks { get; set; }

		[JsonPropertyName("id")]
		public long RawId { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("artwork_url")]
		public string ArtworkUrl { get; set; }

		[JsonPropertyName("user")]
		public SoundCloudUser User { get; set; }
	}
}