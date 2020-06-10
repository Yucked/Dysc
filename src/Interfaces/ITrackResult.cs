namespace Dysc.Interfaces {
	/// <summary>
	/// 
	/// </summary>
	public interface ITrackResult {
		/// <summary>
		/// 
		/// </summary>
		string Id { get; }

		/// <summary>
		/// 
		/// </summary>
		string Title { get; }

		/// <summary>
		/// 
		/// </summary>
		string Url { get; }

		/// <summary>
		/// 
		/// </summary>
		ISourceAuthor Author { get; }

		/// <summary>
		/// 
		/// </summary>
		long Duration { get; }

		/// <summary>
		/// 
		/// </summary>
		bool IsStreamable { get; }

		/// <summary>
		/// 
		/// </summary>
		string ArtworkUrl { get; }

		/// <summary>
		/// 
		/// </summary>
		string StreamUrl { get; }
	}
}