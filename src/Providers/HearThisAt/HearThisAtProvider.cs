using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dysc.Interfaces;
using Dysc.Providers.HearThisAt.Entities;
using Dysc.Stream;

namespace Dysc.Providers.HearThisAt {
    /// <inheritdoc />
    public sealed class HearThisAtProvider : ISourceProvider {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 
        /// </summary>
        public const string URL = "https://hearthis.at/";

        /// <summary>
        /// 
        /// </summary>
        public const string API_URL = "https://api-v2.hearthis.at/";

        /// <summary>
        /// </summary>
        /// <param name="httpClient"></param>
        public HearThisAtProvider(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async ValueTask<IReadOnlyList<ITrackResult>> SearchAsync(string query) {
            var requestUrl = API_URL
                .WithPath("search")
                .WithParameter("t", WebUtility.UrlEncode(query))
                .WithParameter("count", "10");

            var hearThisTracks = await _httpClient
                .ReadFromJsonAsync<IReadOnlyList<HearThisTrack>>(requestUrl)
                .ConfigureAwait(false);
            return hearThisTracks;
        }

        /// <inheritdoc />
        public async ValueTask<ITrackResult> GetTrackAsync(string query) {
            Guard.NotNull(nameof(query), query);
            Guard.IsValidUrl(nameof(query), query);

            var hearThisTrack = await _httpClient
                .ReadFromJsonAsync<HearThisTrack>(query.Replace(URL, API_URL))
                .ConfigureAwait(false);
            return hearThisTrack;
        }

        /// <inheritdoc />
        public async ValueTask<IPlaylistResult> GetPlaylistAsync(string query) {
            Guard.NotNull(nameof(query), query);
            Guard.IsValidUrl(nameof(query), query);

            async Task<HearThisPlaylist> GetPlaylistInfoAsync() {
                var urlSplits = query.Remove(0, URL.Length).Split('/');
                var artistName = urlSplits[0];
                var playlistName = (string.IsNullOrWhiteSpace(urlSplits[^1]) ? urlSplits[^2] : urlSplits[^1])
                    .Replace('-', ' ');

                var playlistUrl = API_URL
                    .WithPath(artistName)
                    .WithParameter("type", "playlists");

                var hearThisPlaylists =
                    await _httpClient.ReadFromJsonAsync<IReadOnlyCollection<HearThisPlaylist>>(playlistUrl);
                var defaultPlaylist = default(HearThisPlaylist);
                foreach (var playlist in hearThisPlaylists) {
                    var lowerPlaylist = playlist.Title.ToLower();
                    if (lowerPlaylist == playlistName) {
                        defaultPlaylist = playlist;
                        break;
                    }

                    if (lowerPlaylist.Contains(playlistName)) {
                        defaultPlaylist = playlist;
                        break;
                    }

                    if (!playlistName.Contains(lowerPlaylist)) {
                        continue;
                    }

                    defaultPlaylist = playlist;
                    break;
                }

                return defaultPlaylist ?? new HearThisPlaylist();
            }

            var hearThisPlaylist = await GetPlaylistInfoAsync();
            var requestUrl = query.Replace(URL, API_URL);
            var hearThisTracks = await _httpClient.ReadFromJsonAsync<IReadOnlyList<HearThisTrack>>(requestUrl);

            hearThisPlaylist.Duration = hearThisTracks.Sum(x => int.Parse(x.RawDuration));
            hearThisPlaylist.Tracks = hearThisTracks;
            hearThisPlaylist.Url ??= query;

            return hearThisPlaylist;
        }

        /// <inheritdoc />
        public async ValueTask<PipedStream> GetTrackStreamAsync(string trackIdentifier) {
            Guard.NotNull(nameof(trackIdentifier), trackIdentifier);
            Guard.IsValidUrl(nameof(trackIdentifier), trackIdentifier);

            if (trackIdentifier.AsUri().Segments[^1] != "listen") {
                trackIdentifier = trackIdentifier.WithPath("listen");
            }

            var pipedStream = await _httpClient.GetPipedStreamAsync(trackIdentifier);
            return pipedStream;
        }

        /// <inheritdoc />
        public ValueTask<PipedStream> GetTrackStreamAsync(ITrackResult trackResult) {
            return GetTrackStreamAsync(trackResult.Url);
        }
    }
}