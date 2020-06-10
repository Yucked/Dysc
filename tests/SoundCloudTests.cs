using System.Threading.Tasks;
using Dysc.Providers.SoundCloud;
using Dysc.Tests.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dysc.Tests {
	[TestClass]
	public sealed class SoundCloudTests : IProviderTest {
		private readonly SoundCloudProvider _provider;

		public SoundCloudTests() {
			var serviceCollection = new ServiceCollection()
			   .AddHttpClient()
			   .AddSingleton<SoundCloudProvider>();

			var provider = serviceCollection.BuildServiceProvider();
			_provider = provider.GetRequiredService<SoundCloudProvider>();
		}

		[DataTestMethod]
		[DataRow("SiR Hair Down")]
		[DataRow("J. Cole No Role Modelz")]
		[DataRow("The Weeknd Wicked Games")]
		[DataRow("Eminem Lucky You")]
		[DataRow("Logic OCD")]
		public async Task SearchAsync(string query) {
			var trackResults = await _provider.SearchAsync(query)
			   .ConfigureAwait(false);

			Assert.IsNotNull(trackResults);
			Assert.IsTrue(trackResults.Count != 0);
			
			foreach (var track in trackResults) {
				track.IsValidTrack();
				track.Author.IsValidAuthor();
			}
		}


		[DataTestMethod]
		[DataRow("https://soundcloud.com/theweeknd/hurt-you")]
		[DataRow("https://soundcloud.com/gesaffelstein/lost-in-the-fire")]
		[DataRow("https://soundcloud.com/kanyewest/i-love-it-freaky-girl-edit")]
		[DataRow("https://soundcloud.com/theweeknd/blinding-lights")]
		[DataRow("https://soundcloud.com/theweeknd/heartless")]
		public async Task GetTrackAsync(string trackUrl) {
			var track = await _provider.GetTrackAsync(trackUrl)
			   .ConfigureAwait(false);

			track.IsValidTrack();
			track.Author.IsValidAuthor();
		}

		[DataTestMethod]
		[DataRow("https://soundcloud.com/kanyewest/sets/ye-49")]
		[DataRow("https://soundcloud.com/albsoon/sets/the-weeknd-more-balloons-remixed-by-sango")]
		[DataRow("https://soundcloud.com/jojiofficial/sets/ballads-1-3")]
		[DataRow("https://soundcloud.com/logic_official/sets/confessions-of-a-dangerous-3")]
		[DataRow("https://soundcloud.com/eminemofficial/sets/kamikaze-34")]
		public async Task GetPlaylistAsync(string playlistUrl) {
			var playlist = await _provider.GetPlaylistAsync(playlistUrl)
			   .ConfigureAwait(false);

			playlist.IsValidPlaylist();
			playlist.Author.IsValidAuthor();
			
			foreach (var track in playlist.Tracks) {
				track.IsValidTrack();
				track.Author.IsValidAuthor();
			}
		}

		[DataTestMethod]
		[DataRow("https://soundcloud.com/theweeknd/hurt-you")]
		[DataRow("https://soundcloud.com/gesaffelstein/lost-in-the-fire")]
		[DataRow("https://soundcloud.com/kanyewest/i-love-it-freaky-girl-edit")]
		[DataRow("https://soundcloud.com/theweeknd/blinding-lights")]
		[DataRow("https://soundcloud.com/theweeknd/heartless")]
		public async Task GetStreamAsync(string trackUrl) {
			var stream = await _provider.GetTrackStreamAsync(trackUrl);
			Assert.IsNotNull(stream);
		}
	}
}