using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dysc.Infos;
using Dysc.Interfaces;
using Dysc.Providers.YouTube.API;
using Dysc.Providers.YouTube.Entities;
using Dysc.Search;

namespace Dysc.Providers.YouTube {
    /// <summary>
    /// </summary>
    public sealed class YouTubeProvider : IAudioProvider {
		private readonly HttpClient _httpClient;
		private readonly YouTubeParser _parser;

        /// <summary>
        /// </summary>
        /// <param name="httpClient"></param>
        public YouTubeProvider(HttpClient httpClient) {
			_httpClient = httpClient;
			_parser = new YouTubeParser(_httpClient);
		}

        /// <summary>
        /// </summary>
        /// <param name="clientFactory"></param>
        public YouTubeProvider(IHttpClientFactory clientFactory)
			: this(clientFactory.CreateClient(nameof(YouTubeProvider))) {
		}

		/// <inheritdoc />
		public async ValueTask<SearchResponse> SearchAsync(string query) {
			var searchResponse = new SearchResponse {
				Query = query
			};

			YouTubeParser.ParseId(query, out var videoId, out var playlistId);
			string url;
			switch (query) {
				case var linkUrl when Uri.IsWellFormedUriString(query, UriKind.Absolute):
					if (linkUrl.Contains("list=")) {
						url = YouTubeParser.BASE_URL
						   .WithPath("list_ajax")
						   .WithParameter("style", "json")
						   .WithParameter("action_get_list", "1")
						   .WithParameter("list", playlistId);

						searchResponse.Status = SearchStatus.PlaylistLoaded;
					}
					else {
						url = YouTubeParser.BASE_URL
						   .WithPath("search_ajax")
						   .WithParameter("style", "json")
						   .WithParameter("search_query", videoId);

						searchResponse.Status = SearchStatus.TrackLoaded;
					}

					break;

				default:
					url = YouTubeParser.BASE_URL
					   .WithPath("search_ajax")
					   .WithParameter("style", "json")
					   .WithParameter("search_query", WebUtility.UrlEncode(query));

					searchResponse.Status = SearchStatus.SearchResult;
					break;
			}

			var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
			var responseMessage = await _httpClient.SendAsync(requestMessage)
			   .ConfigureAwait(false);

			if (!responseMessage.IsSuccessStatusCode) {
				return searchResponse.WithError();
			}

			var byteArray = await responseMessage.Content
			   .ReadAsByteArrayAsync()
			   .ConfigureAwait(false);

			switch (searchResponse.Status) {
				case SearchStatus.PlaylistLoaded: {
					var youTubePlaylist = JsonSerializer.Deserialize<YouTubePlaylist>(byteArray);
					searchResponse.Playlist = youTubePlaylist.BuildPlaylistInfo(url);
					searchResponse.AddTracks(youTubePlaylist.Videos.Select(x => x.ToTrackInfo));
				}
					break;

				case SearchStatus.SearchResult: {
					var youTubeSearch = JsonSerializer.Deserialize<YouTubeSearch>(byteArray);
					searchResponse.AddTracks(youTubeSearch.Video.Select(x => x.ToTrackInfo));
				}
					break;

				case SearchStatus.TrackLoaded: {
					var youTubeSearch = JsonSerializer.Deserialize<YouTubeSearch>(byteArray);
					var youTubeVideo = youTubeSearch.Video.FirstOrDefault(x => x.Id == videoId);
					searchResponse.AddTrack(youTubeVideo.ToTrackInfo);
				}
					break;
			}

			return searchResponse.Tracks.Count == 0
				? searchResponse.WithNoMatches()
				: searchResponse;
		}

		/// <inheritdoc />
		public async ValueTask<Stream> GetStreamAsync(string trackUrl) {
			YouTubeParser.ParseId(trackUrl, out var videoId, out _);
			var videoInfo = await _parser.GetVideoInfoAsync(videoId);

			if (!videoInfo.TryGetValue("player_response", out var playerResponseValue)) {
				throw new Exception("Couldn't find player response key in JSON.");
			}

			var playerResponse = JsonSerializer.Deserialize<PlayerResponse>(playerResponseValue);
			if (playerResponse.Playability.Status != "OK") {
				throw new Exception($"Requested stream for {trackUrl} is {playerResponse.Playability.Status}.");
			}

			var adaptiveFormats = playerResponse.Streaming.AdaptiveFormats;
			var bestFormat = adaptiveFormats
			   .Where(x => x.MimeType.Contains("audio") && x.MimeType.Contains("opus") && !string.IsNullOrWhiteSpace(x.Url))
			   .OrderByDescending(x => x.Bitrate)
			   .FirstOrDefault(x => x.AudioChannels == 2 && x.AudioSampleRate == "48000");

			var stream = _httpClient.GetBufferedStream(bestFormat.Url, long.Parse(bestFormat.ContentLength), 512);
			return stream;
		}

		/// <inheritdoc />
		public ValueTask<Stream> GetStreamAsync(TrackInfo track) {
			return GetStreamAsync(track.Id);
		}

		/// <inheritdoc />
		public ValueTask DisposeAsync() {
			_httpClient.Dispose();
			return default;
		}
	}
}