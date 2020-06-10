using System.Collections.Generic;

namespace Dysc.Interfaces {
	/// <summary>
	/// 
	/// </summary>
	public interface IPlaylistResult {
		/// <summary>
		/// 
		/// </summary>
		string Title { get; }

		/// <summary>
		/// 
		/// </summary>
		string Id { get; }

		/// <summary>
		/// 
		/// </summary>
		string Url { get; }

		/// <summary>
		/// 
		/// </summary>
		string ArtworkUrl { get; }

		/// <summary>
		/// 
		/// </summary>
		long Duration { get; }

		/// <summary>
		/// 
		/// </summary>
		ISourceAuthor Author { get; }

		/// <summary>
		/// 
		/// </summary>
		IReadOnlyList<ITrackResult> Tracks { get; }
	}
}