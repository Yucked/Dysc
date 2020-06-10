using Dysc.Decoders;
using Dysc.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dysc.Tests {
	public static class TestExtensions {
		public static void IsValidTrack(this ITrackResult trackResult) {
			Assert.IsNotNull(trackResult);

			Assert.IsNotNull(trackResult.Duration);
			Assert.IsTrue(trackResult.Duration != 0);

			Assert.IsNotNull(trackResult.Id);
			Assert.IsNotNull(trackResult.Title);
			Assert.IsNotNull(trackResult.Url);

			// Should be CanBeNull
			// Assert.IsNotNull(trackResult.ArtworkUrl);
			if (trackResult.IsStreamable) {
				Assert.IsNotNull(trackResult.StreamUrl);
			}
			else {
				Assert.IsFalse(trackResult.IsStreamable);
			}

			Assert.IsNotNull(trackResult.Author);
		}

		public static void IsValidAuthor(this ISourceAuthor sourceAuthor) {
			Assert.IsNotNull(sourceAuthor);
			Assert.IsNotNull(sourceAuthor.Id);
			Assert.IsNotNull(sourceAuthor.Name);
			Assert.IsNotNull(sourceAuthor.Url);
			Assert.IsNotNull(sourceAuthor.AvatarUrl);
		}

		public static void IsValidPlaylist(this IPlaylistResult playlistResult) {
			Assert.IsNotNull(playlistResult);
			Assert.IsNotNull(playlistResult.Tracks);
			Assert.IsNotNull(playlistResult.Url);
			Assert.IsNotNull(playlistResult.Duration);
			
			Assert.IsNotNull(playlistResult.Author);
			Assert.IsNotNull(playlistResult.Id);
			Assert.IsNotNull(playlistResult.Title);
			Assert.IsNotNull(playlistResult.ArtworkUrl);
		}

		public static void IsDecodingSuccessfull(this IDecoder decoder) {
			Assert.IsNotNull(decoder);
			Assert.IsTrue(decoder.Channels != 0);
			Assert.IsTrue(decoder.BlockAlign != 0);
			Assert.IsTrue(decoder.SampleRate != 0);
			Assert.IsTrue(decoder.BitsPerSample != 0);
			Assert.IsTrue(decoder.BytesPerSample != 0);
			Assert.IsTrue(decoder.BytesPerSecond != 0);
			Assert.IsNotNull(decoder.Length);
		}
	}
}