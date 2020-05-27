using System.Collections.Generic;
using System.Linq;
using Dysc.Infos;

namespace Dysc.Search {
    /// <summary>
    /// </summary>
    public sealed class SearchResponse {
        /// <summary>
        /// </summary>
        public IReadOnlyCollection<TrackInfo> Tracks
			=> TempTracks;

        /// <summary>
        /// </summary>
        public PlaylistInfo Playlist { get; set; }

        /// <summary>
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// </summary>
        public SearchStatus Status { get; set; }

		private List<TrackInfo> TempTracks { get; set; }

		internal void AddTrack(TrackInfo trackInfo) {
			if (TempTracks == null) {
				TempTracks = new List<TrackInfo>();
			}

			TempTracks.Add(trackInfo);
		}

		internal void AddTracks(IEnumerable<TrackInfo> trackInfos) {
			foreach (var trackInfo in trackInfos) {
				AddTrack(trackInfo);
			}
		}

        /// <summary>
        /// </summary>
        /// <param name="searchResponse"></param>
        /// <returns></returns>
        public bool IsEqual(SearchResponse searchResponse) {
			if (Query.IsStrongMatch(searchResponse.Query) ||
			    Playlist.Url.IsStrongMatch(searchResponse.Playlist.Url)) {
				return true;
			}

			return Tracks.Any(x => searchResponse.Tracks.Any(y => x.Equals(y)));
		}
	}
}