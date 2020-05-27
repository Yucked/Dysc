using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dysc.Infos;
using Dysc.Interfaces;
using Dysc.Providers.BandCamp.Entities;
using Dysc.Search;

namespace Dysc.Providers.BandCamp {
    /// <summary>
    /// </summary>
    public sealed class BandCampProvider : IAudioProvider {
		private readonly HttpClient _httpClient;
		private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// </summary>
        /// <param name="httpClient"></param>
        public BandCampProvider(HttpClient httpClient) {
			_httpClient = httpClient;
			_serializerOptions = new JsonSerializerOptions {
				ReadCommentHandling = JsonCommentHandling.Skip,
				AllowTrailingCommas = false
			};
		}

        /// <summary>
        /// </summary>
        /// <param name="clientFactory"></param>
        public BandCampProvider(IHttpClientFactory clientFactory)
			: this(clientFactory.CreateClient(nameof(BandCampProvider))) {
		}

		/// <inheritdoc />
		public async ValueTask<SearchResponse> SearchAsync(string query) {
			var searchResponse = new SearchResponse {
				Query = query,
				Status = query switch {
					_ when Uri.IsWellFormedUriString(query, UriKind.Absolute)
					       && Patterns.BandCamp.TrackUrl.IsMatch(query)
					=> SearchStatus.TrackLoaded,

					_ when Uri.IsWellFormedUriString(query, UriKind.Absolute)
					       && Patterns.BandCamp.AlbumUrl.IsMatch(query)
					=> SearchStatus.PlaylistLoaded,

					_ when !Uri.IsWellFormedUriString(query, UriKind.Absolute)
					=> SearchStatus.SearchResult
				}
			};

			var url = searchResponse.Status switch {
				SearchStatus.PlaylistLoaded => query,
				SearchStatus.TrackLoaded    => query,
				SearchStatus.SearchResult   => $"https://bandcamp.com/search?q={WebUtility.UrlEncode(query)}"
			};

			using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
			using var responseMessage = await _httpClient.SendAsync(requestMessage)
			   .ConfigureAwait(false);

			if (!responseMessage.IsSuccessStatusCode) {
				return searchResponse.WithError();
			}

			using var content = responseMessage.Content;
			var dataBytes = await content.ReadAsByteArrayAsync()
			   .ConfigureAwait(false);

			if (dataBytes.Length == 0) {
				searchResponse.WithNoMatches();
			}

			switch (searchResponse.Status) {
				case SearchStatus.TrackLoaded:
				case SearchStatus.PlaylistLoaded:
					if (!BandCampParser.TryGetJson(dataBytes, out var json)) {
						return searchResponse.WithError();
					}

					var bcResult = JsonSerializer.Deserialize<BandCampResult>(json, _serializerOptions);
					searchResponse.Status = bcResult.ItemType switch {
						"album" => SearchStatus.PlaylistLoaded,
						"track" => SearchStatus.TrackLoaded,
						_       => SearchStatus.NoMatches
					};

					if (searchResponse.Status == SearchStatus.NoMatches) {
						return searchResponse.WithNoMatches();
					}

					long duration = 0;
					foreach (var trackInfo in bcResult.TrackInfo) {
						var track = trackInfo.AsTrackInfo(bcResult.Artist, bcResult.Url, bcResult.ArtId);
						duration += track.Duration;
						searchResponse.AddTrack(track);
					}

					var playlistInfo = new PlaylistInfo {
						Id = $"{bcResult.Current.Id}",
						Name = bcResult.Current.Title,
						Url = bcResult.Url,
						Duration = duration,
						ArtworkUrl = bcResult.ArtId == 0 ? "" : $"https://f4.bcbits.com/img/a{bcResult.ArtId}_0.jpg"
					};

					searchResponse.Playlist = playlistInfo;
					break;

				case SearchStatus.SearchResult:
					searchResponse.Status = SearchStatus.SearchResult;
					BandCampParser.ScrapeHtml(ref searchResponse, dataBytes);
					break;
			}

			return searchResponse.Tracks.Count == 0
				? searchResponse.WithNoMatches()
				: searchResponse;
		}

		/// <inheritdoc />
		public async ValueTask<Stream> GetStreamAsync(string trackUrl) {
			if (!Uri.IsWellFormedUriString(trackUrl, UriKind.Absolute)) {
				throw new UriFormatException($"{nameof(trackUrl)} must be an absolute url.");
			}

			if (!Patterns.BandCamp.TrackUrl.IsMatch(trackUrl)) {
				throw new Exception($"{nameof(trackUrl)} is not a track url.");
			}

			var requestMessage = new HttpRequestMessage(HttpMethod.Get, trackUrl);
			var responseMessage = await _httpClient.SendAsync(requestMessage)
			   .ConfigureAwait(false);
			var content = responseMessage.Content;
			var dataBytes = await content.ReadAsByteArrayAsync()
			   .ConfigureAwait(false);

			if (!BandCampParser.TryGetJson(dataBytes, out var json)) {
				throw new Exception($"Failed to parse json out of {trackUrl}.");
			}

			var bcResult = JsonSerializer.Deserialize<BandCampResult>(json, _serializerOptions);
			var track = bcResult.TrackInfo.FirstOrDefault();

			if (track == null) {
				throw new Exception($"Failed to fetch stream for following url {trackUrl}.");
			}

			requestMessage = new HttpRequestMessage(HttpMethod.Get, track.File.Mp3Url);
			responseMessage = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead)
			   .ConfigureAwait(false);

			content = responseMessage.Content;
			if (content.Headers.ContentLength.HasValue) {
				return _httpClient.GetBufferedStream(track.File.Mp3Url, content.Headers.ContentLength.Value);
			}

			var stream = await responseMessage.Content.ReadAsStreamAsync()
			   .ConfigureAwait(false);
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