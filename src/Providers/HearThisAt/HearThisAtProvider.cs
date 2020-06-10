using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
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

		/// <summary>
		/// </summary>
		/// <param name="clientFactory"></param>
		public HearThisAtProvider(IHttpClientFactory clientFactory)
			: this(clientFactory.CreateClient(nameof(HearThisAtProvider))) {
		}

		/// <inheritdoc />
		public async ValueTask<IReadOnlyList<ITrackResult>> SearchAsync(string query) {
			var requestUrl = API_URL
			   .WithPath("search")
			   .WithParameter("t", WebUtility.UrlEncode(query))
			   .WithParameter("count", "10");

			var hearThisTracks = await _httpClient
			   .GetFromJsonAsync<IReadOnlyList<HearThisTrack>>(requestUrl)
			   .ConfigureAwait(false);
			return hearThisTracks;
		}

		/// <inheritdoc />
		public async ValueTask<ITrackResult> GetTrackAsync(string query) {
			Guard.NotNull(nameof(query), query);
			Guard.IsValidUrl(nameof(query), query);

			var hearThisTrack = await _httpClient
			   .GetFromJsonAsync<HearThisTrack>(query.Replace(URL, API_URL))
			   .ConfigureAwait(false);
			return hearThisTrack;
		}

		/// <inheritdoc />
		public async ValueTask<IPlaylistResult> GetPlaylistAsync(string query) {
			Guard.NotNull(nameof(query), query);
			Guard.IsValidUrl(nameof(query), query);

			var requestUrl = query.Replace(URL, API_URL);
			var hearThisTracks = await _httpClient.GetFromJsonAsync<IReadOnlyList<HearThisTrack>>(requestUrl)
			   .ConfigureAwait(false);

			return default;
		}

		/// <inheritdoc />
		public async ValueTask<PipedStream> GetTrackStreamAsync(string trackIdentifier) {
			Guard.NotNull(nameof(trackIdentifier), trackIdentifier);
			Guard.IsValidUrl(nameof(trackIdentifier), trackIdentifier);

			var trackUrl = trackIdentifier.Replace(URL, API_URL);
			var responseMessage = await _httpClient.GetAsync(trackUrl)
			   .ConfigureAwait(false);

			Guard.IsSuccessfulResponse(responseMessage);

			return default;
		}

		/// <inheritdoc />
		public ValueTask<PipedStream> GetTrackStreamAsync(ITrackResult trackResult) {
			return GetTrackStreamAsync(trackResult.Url);
		}
	}
}