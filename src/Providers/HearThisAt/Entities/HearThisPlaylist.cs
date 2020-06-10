using System.Collections.Generic;
using Dysc.Interfaces;

namespace Dysc.Providers.HearThisAt.Entities {
	internal class HearThisPlaylist : IPlaylistResult{
		public string Title { get; }
		public string Id { get; }
		public string Url { get; }
		public string ArtworkUrl { get; }
		public long Duration { get; }
		public ISourceAuthor Author { get; }
		public IReadOnlyList<ITrackResult> Tracks { get; }
	}
}