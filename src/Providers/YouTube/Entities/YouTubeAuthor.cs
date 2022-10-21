using Dysc.Interfaces;

namespace Dysc.Providers.YouTube.Entities {
	/// <inheritdoc />
	public class YouTubeAuthor : ISourceAuthor {
		/// <inheritdoc />
		public string Name { get; set; }

		/// <inheritdoc />
		public string Id { get; set; }

		/// <inheritdoc />
		public string AvatarUrl { get; set; }

		/// <inheritdoc />
		public string Url { get; set; }
	}
}