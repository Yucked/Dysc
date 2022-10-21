using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Dysc.Interfaces;
using Dysc.Providers.YouTube.Entities;
using Dysc.Stream;

namespace Dysc.Providers.YouTube {
	/// <inheritdoc />
	public sealed class YouTubeProvider : ISourceProvider {
		/// <summary>
		/// 
		/// </summary>
		public const string URL = "https://youtube.com/";

		/// <summary>
		/// 
		/// </summary>
		public const string API_URL = "https://youtube.googleapis.com/v/";

		private readonly HttpClient _httpClient;

		/// <inheritdoc />
		public YouTubeProvider(IHttpClientFactory httpClientFactory)
			: this(httpClientFactory.CreateClient(nameof(YouTubeProvider))) {
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpClient"></param>
		public YouTubeProvider(HttpClient httpClient) {
			_httpClient = httpClient;
		}

		/// <inheritdoc />
		public async ValueTask<IReadOnlyList<ITrackResult>> SearchAsync(string query) {
			Guard.NotNull(nameof(query), query);
			if (Uri.IsWellFormedUriString(query, UriKind.Absolute)) {
				throw new ArgumentException("", nameof(query));
			}

			var requestUrl = URL
			   .WithPath("search_ajax")
			   .WithParameter("style", "json")
			   .WithParameter("search_query", query);

			var youTubeVideos = await _httpClient
			   .GetTokenFromJsonAsync<IReadOnlyList<YouTubeVideo>>(requestUrl, "video");

			return youTubeVideos;
		}

		/// <inheritdoc />
		public async ValueTask<ITrackResult> GetTrackAsync(string query) {
			Guard.NotNull(nameof(query), query);
			Guard.IsValidUrl(nameof(query), query);

			var requestUrl = URL
			   .WithPath("search_ajax")
			   .WithParameter("style", "json")
			   .WithParameter("search_query", query);

			var youTubeVideo = await _httpClient
			   .ReadFromJsonAsync<YouTubeVideo>(requestUrl);

			return youTubeVideo;
		}

		/// <inheritdoc />
		public async ValueTask<IPlaylistResult> GetPlaylistAsync(string query) {
			Guard.NotNull(nameof(query), query);
			Guard.IsValidUrl(nameof(query), query);

			YouTubeParser.ParseId(query, out _, out var playlistId);
			var requestUrl = URL
			   .WithPath("list_ajax")
			   .WithParameter("style", "json")
			   .WithParameter("action_get_list", "1")
			   .WithParameter("list", playlistId);

			var youTubeVideo = await _httpClient
			   .ReadFromJsonAsync<YouTubeVideo>(requestUrl);

			return default;
		}

		/// <inheritdoc />
		public async ValueTask<PipedStream> GetTrackStreamAsync(string trackIdentifier) {
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public ValueTask<PipedStream> GetTrackStreamAsync(ITrackResult trackResult) {
			return GetTrackStreamAsync(trackResult.Url);
		}
	}
}