using System.Net.Http;
using Dysc.Decoders;

namespace Dysc.Stream {
	using System.IO;

	/// <summary>
	/// 
	/// </summary>
	public sealed class StreamOptions {
		/// <summary>
		/// 
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public long Length { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Stream Stream { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public HttpClient HttpClient { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public IDecoder Decoder { get; set; }
	}
}