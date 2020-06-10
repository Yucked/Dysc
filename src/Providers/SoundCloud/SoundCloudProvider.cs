using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Dysc.Interfaces;
using Dysc.Providers.SoundCloud.Entities;
using Dysc.Stream;

namespace Dysc.Providers.SoundCloud {
	/// <inheritdoc />
	public sealed class SoundCloudProvider : ISourceProvider {
		/// <summary>
		/// 
		/// </summary>
		public const string API_URL = "https://api-v2.soundcloud.com";

		private readonly SoundCloudParser _cloudParser;
		private readonly HttpClient _httpClient;

		/// <summary>
		/// </summary>
		/// <param name="httpClient"></param>
		public SoundCloudProvider(HttpClient httpClient) {
			_httpClient = httpClient;
			_cloudParser = new SoundCloudParser(_httpClient);


			_ = _cloudParser.ValidateClientIdAsync();
		}

		/// <inheritdoc />
		public SoundCloudProvider(IHttpClientFactory httpClientFactory)
			: this(httpClientFactory.CreateClient(nameof(SoundCloudProvider))) {
		}

		/// <inheritdoc />
		public async ValueTask<IReadOnlyList<ITrackResult>> SearchAsync(string query) {
			Guard.NotNull(nameof(query), query);

			await _cloudParser.ValidateClientIdAsync();
			var requestUrl = API_URL
			   .WithPath("search")
			   .WithParameter("q", query)
			   .WithParameter("client_id", SoundCloudParser.ClientId);

			var soundCloudSearch = await _httpClient.GetFromJsonAsync<SoundCloudSearch>(requestUrl)
			   .ConfigureAwait(false);
			return soundCloudSearch.Collection;
		}

		/// <inheritdoc />
		public async ValueTask<ITrackResult> GetTrackAsync(string query) {
			Guard.NotNull(nameof(query), query);
			Guard.IsValidUrl(nameof(query), query);

			await _cloudParser.ValidateClientIdAsync();
			var requestUrl = API_URL
			   .WithPath("resolve")
			   .WithParameter("url", query)
			   .WithParameter("client_id", SoundCloudParser.ClientId);

			var soundCloudTrack = await _httpClient.GetFromJsonAsync<SoundCloudTrack>(requestUrl);
			return soundCloudTrack;
		}

		/// <inheritdoc />
		public async ValueTask<IPlaylistResult> GetPlaylistAsync(string query) {
			Guard.NotNull(nameof(query), query);
			Guard.IsValidUrl(nameof(query), query);

			await _cloudParser.ValidateClientIdAsync();
			var requestUrl = API_URL
			   .WithPath("resolve")
			   .WithParameter("url", query)
			   .WithParameter("client_id", SoundCloudParser.ClientId);

			var soundCloudPlaylist = await _httpClient.ReadFromJsonAsync<SoundCloudPlaylist>(requestUrl);
			return soundCloudPlaylist;
		}

		/// <inheritdoc />
		public async ValueTask<PipedStream> GetTrackStreamAsync(string trackIdentifier) {
			Guard.NotNull(nameof(trackIdentifier), trackIdentifier);
			Guard.IsValidUrl(nameof(trackIdentifier), trackIdentifier);

			await _cloudParser.ValidateClientIdAsync();
			var queryUrl = API_URL
			   .WithPath("resolve")
			   .WithParameter("url", trackIdentifier)
			   .WithParameter("client_id", SoundCloudParser.ClientId);

			var soundCloudTrack = await _httpClient.GetFromJsonAsync<SoundCloudTrack>(queryUrl);
			var streamUrl = ((ITrackResult) soundCloudTrack)
			   .StreamUrl
			   .WithParameter("client_id", SoundCloudParser.ClientId);

			var url = await _httpClient.GetTokenFromJsonAsync<string>(streamUrl, "url");
			var pipedStream = await _httpClient.GetPipedStreamAsync(url);
			return pipedStream;
		}

		/// <inheritdoc />
		public ValueTask<PipedStream> GetTrackStreamAsync(ITrackResult trackResult) {
			return GetTrackStreamAsync(trackResult.Url);
		}
	}
}