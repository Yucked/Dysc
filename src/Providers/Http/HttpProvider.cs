using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Dysc.Interfaces;
using Dysc.Stream;

namespace Dysc.Providers.Http {
	/// <inheritdoc />
	public sealed class HttpProvider : ISourceProvider {
		private readonly HttpClient _httpClient;

		/// <summary>
		/// </summary>
		/// <param name="httpClient"></param>
		public HttpProvider(HttpClient httpClient) {
			_httpClient = httpClient;
		}

		/// <summary>
		/// </summary>
		/// <param name="clientFactory"></param>
		public HttpProvider(IHttpClientFactory clientFactory)
			: this(clientFactory.CreateClient(nameof(HttpProvider))) {
		}

		/// <inheritdoc />
		public ValueTask<IReadOnlyList<ITrackResult>> SearchAsync(string query) {
			throw new NotSupportedException($"{nameof(HttpProvider)} doesn't support {nameof(SearchAsync)}.");
		}

		/// <inheritdoc />
		public ValueTask<ITrackResult> GetTrackAsync(string query) {
			throw new NotSupportedException($"{nameof(HttpProvider)} doesn't support {nameof(GetTrackAsync)}.");
		}

		/// <inheritdoc />
		public ValueTask<IPlaylistResult> GetPlaylistAsync(string query) {
			throw new NotSupportedException($"{nameof(HttpProvider)} doesn't support {nameof(GetPlaylistAsync)}.");
		}

		/// <inheritdoc />
		public async ValueTask<PipedStream> GetTrackStreamAsync(string trackIdentifier) {
			Guard.NotNull(nameof(trackIdentifier), trackIdentifier);
			Guard.IsValidUrl(nameof(trackIdentifier), trackIdentifier);

			var pipedStream = await _httpClient.GetPipedStreamAsync(trackIdentifier);
			return pipedStream;
		}

		/// <inheritdoc />
		public ValueTask<PipedStream> GetTrackStreamAsync(ITrackResult trackResult) {
			throw new NotSupportedException($"{nameof(HttpProvider)} doesn't support {nameof(GetTrackStreamAsync)}.");
		}
	}
}