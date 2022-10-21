using System.Collections.Generic;
using System.Net.Http;
using Dysc.Interfaces;

namespace Dysc {
	/// <summary>
	/// 
	/// </summary>
	public sealed class Dysk {
		private readonly IDictionary<SourceProvider, ISourceProvider> _sourceProviders;

		/// <inheritdoc cref="IHttpClientFactory" />
		public Dysk(IHttpClientFactory clientFactory) {
			
		}

		/*
		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpClient"></param>
		public Dysk(HttpClient httpClient) {
			_sourceProviders = new Dictionary<SourceProvider, ISourceProvider> {
				{ SourceProvider.Http, new HttpProvider(httpClient) },
				{ SourceProvider.SoundCloud, new SoundCloudProvider(httpClient) },
				{ SourceProvider.HearThisAt, new HearThisAtProvider(httpClient) }
			};
		}*/

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sourceProvider"></param>
		/// <returns></returns>
		public ISourceProvider GetProvider(SourceProvider sourceProvider) {
			return _sourceProviders[sourceProvider];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sourceProvider"></param>
		/// <param name="provider"></param>
		/// <returns></returns>
		public bool TryGetSource(SourceProvider sourceProvider, out ISourceProvider provider) {
			return _sourceProviders.TryGetValue(sourceProvider, out provider);
		}
	}
}