using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dysc.Stream;

namespace Dysc.Interfaces {
	/// <summary>
	/// 
	/// </summary>
	public interface ISourceProvider {
		/// <summary>
		/// Returns a collection of <see cref="ITrackResult"/> based on <paramref name="query"/>.
		/// </summary>
		/// <param name="query"></param>
		/// <returns><see cref="ICollection{ITrackResult}"/></returns>
		/// <exception cref="ArgumentNullException">Throws if <paramref name="query"/> is null/empty/whitespace.</exception>
		/// <exception cref="ArgumentException">Throws if <paramref name="query"/> is in invalid format.</exception>
		/// <exception cref="NotSupportedException">Throws if provider doesn't support method.</exception>
		ValueTask<IReadOnlyList<ITrackResult>> SearchAsync(string query);

		/// <summary>
		/// Returns <see cref="ITrackResult"/> based on <paramref name="query"/>.
		/// </summary>
		/// <param name="query"></param>
		/// <returns><see cref="ITrackResult"/></returns>
		/// <exception cref="ArgumentNullException">Throws if <paramref name="query"/> is null/empty/whitespace.</exception>
		/// <exception cref="ArgumentException">Throws if <paramref name="query"/> is in invalid format.</exception>
		/// <exception cref="NotSupportedException">Throws if provider doesn't support method.</exception>
		ValueTask<ITrackResult> GetTrackAsync(string query);

		/// <summary>
		/// Returns <see cref="IPlaylistResult"/> based on <paramref name="query"/>.
		/// </summary>
		/// <param name="query"></param>
		/// <returns><see cref="IPlaylistResult"/></returns>
		/// <exception cref="ArgumentNullException">Throws if <paramref name="query"/> is null/empty/whitespace.</exception>
		/// <exception cref="ArgumentException">Throws if <paramref name="query"/> is in invalid format.</exception>
		/// <exception cref="NotSupportedException">Throws if provider doesn't support method.</exception>
		ValueTask<IPlaylistResult> GetPlaylistAsync(string query);

		/// <summary>
		/// Returns either a complete <seealso cref="PipedStream"/> or semi-buffered <see cref="PipedStream"/>.
		/// </summary>
		/// <param name="trackIdentifier"></param>
		/// <returns><see cref="PipedStream"/></returns>
		/// <exception cref="ArgumentNullException">Throws if <paramref name="trackIdentifier"/> is null/empty/whitespace.</exception>
		/// <exception cref="NotSupportedException">Throws if provider doesn't support method.</exception>
		ValueTask<PipedStream> GetTrackStreamAsync(string trackIdentifier);

		/// <summary>
		/// Returns either a complete <seealso cref="PipedStream"/> or semi-buffered <see cref="PipedStream"/>. 
		/// </summary>
		/// <param name="trackResult"></param>
		/// <returns><see cref="PipedStream"/></returns>
		/// <exception cref="ArgumentNullException">Throws if <paramref name="trackResult"/> is null/empty/whitespace.</exception>
		/// <exception cref="NotSupportedException">Throws if provider doesn't support method.</exception>
		ValueTask<PipedStream> GetTrackStreamAsync(ITrackResult trackResult);
	}
}