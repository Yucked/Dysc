using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dysc.Infos;
using Dysc.Interfaces;
using Dysc.Providers.HearThis.At.Entities;
using Dysc.Search;

namespace Dysc.Providers.HearThis.At {
    /// <summary>
    /// </summary>
    public sealed class HearThisAtProvider : IAudioProvider {
        private const string BASE_URL = "https://api-v2.hearthis.at/";
        private const string WEB_URL = "https://hearthis.at/";
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        public HearThisAtProvider(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        /// <summary>
        /// </summary>
        /// <param name="clientFactory"></param>
        public HearThisAtProvider(IHttpClientFactory clientFactory)
            : this(clientFactory.CreateClient(nameof(HearThisAtProvider))) { }

        /// <inheritdoc />
        public async ValueTask<SearchResponse> SearchAsync(string query) {
            var searchResponse = new SearchResponse {
                Query = query
            };

            string url;
            switch (query) {
                case var q when Uri.IsWellFormedUriString(query, UriKind.Absolute):
                    url = q.Replace(WEB_URL, BASE_URL);
                    searchResponse.Status = q.Contains("set")
                        ? SearchStatus.PlaylistLoaded
                        : SearchStatus.TrackLoaded;
                    break;
                default:
                    url = BASE_URL
                        .WithPath("search")
                        .WithParameter("t", WebUtility.UrlEncode(query))
                        .WithParameter("count", "10");
                    searchResponse.Status = SearchStatus.SearchResult;
                    break;
            }

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            using var responseMessage = await _httpClient
                .ChangeUserAgent()
                .SendAsync(requestMessage)
                .ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
                return searchResponse.WithError();

            var byteArray = await responseMessage.Content
                .ReadAsByteArrayAsync()
                .ConfigureAwait(false);

            if (byteArray.Length == 0 || byteArray.Length == 69)
                return searchResponse.WithError();

            switch (searchResponse.Status) {
                case SearchStatus.PlaylistLoaded:
                    var playlistTracks = JsonSerializer.Deserialize<ICollection<HearThisTrack>>(byteArray);
                    searchResponse.AddTracks(playlistTracks.Select(x => x.ToTrackInfo));
                    break;

                case SearchStatus.SearchResult:
                    var tracks = JsonSerializer.Deserialize<ICollection<HearThisTrack>>(byteArray);
                    searchResponse.AddTracks(tracks.Select(x => x.ToTrackInfo));
                    break;

                case SearchStatus.TrackLoaded:
                    var track = JsonSerializer.Deserialize<HearThisTrack>(byteArray);
                    searchResponse.AddTrack(track.ToTrackInfo);
                    break;
            }

            return searchResponse.Tracks.Count == 0
                ? searchResponse.WithNoMatches()
                : searchResponse;
        }

        /// <inheritdoc />
        public async ValueTask<Stream> GetStreamAsync(string trackId) {
            if (!Uri.IsWellFormedUriString(trackId, UriKind.Absolute))
                throw new UriFormatException($"{nameof(trackId)} must be an absolute URL string.");

            var url = trackId.Replace(WEB_URL, BASE_URL);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            var responseMessage = await _httpClient.SendAsync(requestMessage)
                .ConfigureAwait(false);

            var byteArray = await responseMessage.Content
                .ReadAsByteArrayAsync()
                .ConfigureAwait(false);

            var track = JsonSerializer.Deserialize<HearThisTrack>(byteArray);
            var stream = _httpClient.GetBufferedStream(track.StreamUrl, long.Parse(track.Duration));
            return stream;
        }

        /// <inheritdoc />
        public ValueTask<Stream> GetStreamAsync(TrackInfo track) {
            return GetStreamAsync(track.Url);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync() {
            _httpClient.Dispose();
            return default;
        }
    }
}