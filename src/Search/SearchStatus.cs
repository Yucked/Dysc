namespace Dysc.Search {
    /// <summary>
    /// </summary>
    public enum SearchStatus {
        /// <summary>
        ///     Nothing was found for your query.
        /// </summary>
        NoMatches = 0,

        /// <summary>
        ///     Something went wrong internally when searching.
        /// </summary>
        SearchError = 1,

        /// <summary>
        ///     Search results for your raw search query.
        /// </summary>
        SearchResult = 2,

        /// <summary>
        ///     A single track was returned based on url.
        /// </summary>
        TrackLoaded = 3,

        /// <summary>
        ///     Playlist with all it's tracks was returned based on url.
        /// </summary>
        PlaylistLoaded = 4
	}
}