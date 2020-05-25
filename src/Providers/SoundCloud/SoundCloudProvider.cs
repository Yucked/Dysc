using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dysc.Infos;
using Dysc.Interfaces;
using Dysc.Providers.SoundCloud.Entities;
using Dysc.Search;

namespace Dysc.Providers.SoundCloud {
    /// <summary>
    /// </summary>
    public sealed class SoundCloudProvider : IAudioProvider {
        private const string BASE_URL = "https://api.soundcloud.com";
        private readonly SoundCloudParser _cloudParser;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        public SoundCloudProvider(HttpClient httpClient) {
            _httpClient = httpClient;
            _cloudParser = new SoundCloudParser(_httpClient);
        }

        /// <summary>
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public SoundCloudProvider(IHttpClientFactory httpClientFactory)
            : this(httpClientFactory.CreateClient(nameof(SoundCloudProvider))) { }

        /// <inheritdoc />
        public async ValueTask<SearchResponse> SearchAsync(string query) {
            await _cloudParser.ValidateClientIdAsync()
                .ConfigureAwait(false);

            var searchResponse = new SearchResponse {
                Query = query
            };

            var url = "";
            switch (query) {
                case var q when Uri.IsWellFormedUriString(query, UriKind.Absolute):
                    if (!q.Contains("sets")) {
                        url = BASE_URL
                            .WithPath("resolve")
                            .WithParameter("url", query)
                            .WithParameter("client_id", SoundCloudParser.ClientId);

                        searchResponse.Status = SearchStatus.TrackLoaded;
                    }
                    else {
                        url = BASE_URL
                            .WithPath("resolve")
                            .WithParameter("url", query)
                            .WithParameter("client_id", SoundCloudParser.ClientId);

                        searchResponse.Status = SearchStatus.PlaylistLoaded;
                    }

                    break;

                case var _ when !Uri.IsWellFormedUriString(query, UriKind.Absolute):
                    url = BASE_URL
                        .WithPath("tracks")
                        .WithParameter("q", query)
                        .WithParameter("client_id", SoundCloudParser.ClientId);

                    searchResponse.Status = SearchStatus.SearchResult;
                    break;
            }

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            using var responseMessage = await _httpClient.SendAsync(requestMessage)
                .ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
                return searchResponse.WithError();

            var byteArray = await responseMessage.Content
                .ReadAsByteArrayAsync()
                .ConfigureAwait(false);

            if (byteArray.Length == 0)
                return searchResponse.WithError();

            switch (searchResponse.Status) {
                case SearchStatus.TrackLoaded:
                    var scTrack = JsonSerializer.Deserialize<SoundCloudTrack>(byteArray);
                    searchResponse.AddTrack(scTrack.ToTrackInfo);
                    break;

                case SearchStatus.PlaylistLoaded:
                    var scPly = JsonSerializer.Deserialize<SoundCloudPlaylist>(byteArray);
                    searchResponse.Playlist = scPly.ToPlaylistInfo;
                    searchResponse.AddTracks(scPly.Tracks.Select(x => x.ToTrackInfo));

                    break;

                case SearchStatus.SearchResult:
                    var scTracks = JsonSerializer.Deserialize<IEnumerable<SoundCloudTrack>>(byteArray);
                    searchResponse.AddTracks(scTracks.Select(x => x.ToTrackInfo));
                    break;
            }

            return searchResponse.Tracks.Count != 0
                ? searchResponse
                : searchResponse.WithNoMatches();
        }

        /// <inheritdoc />
        public async ValueTask<Stream> GetStreamAsync(string trackUrl) {
            if (!Uri.IsWellFormedUriString(trackUrl, UriKind.Absolute))
                throw new UriFormatException($"{nameof(trackUrl)} must be an absolute uri string.");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, trackUrl);
            var responseMessage = await _httpClient.SendAsync(requestMessage)
                .ConfigureAwait(false);

            var byteArray = await responseMessage.Content.ReadAsByteArrayAsync()
                .ConfigureAwait(false);

            var transcoding = SoundCloudParser.FetchBestTranscoding(byteArray);
            requestMessage = new HttpRequestMessage(HttpMethod.Get, transcoding.Url);
            responseMessage = await _httpClient.SendAsync(requestMessage)
                .ConfigureAwait(false);

            byteArray = await responseMessage.Content.ReadAsByteArrayAsync()
                .ConfigureAwait(false);

            if (!JsonDocument.Parse(byteArray).RootElement.TryGetProperty("url", out var url))
                throw new Exception("SoundCloud API did not return track's stream url.");

            requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{url}");
            responseMessage = await _httpClient.SendAsync(requestMessage)
                .ConfigureAwait(false);

            var stream = await responseMessage.Content.ReadAsStreamAsync()
                .ConfigureAwait(false);
            stream.Position = 0;

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