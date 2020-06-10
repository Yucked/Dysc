using System.Threading.Tasks;
using Dysc.Providers.HearThisAt;
using Dysc.Tests.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dysc.Tests {
	[TestClass]
	public sealed class HearThisAtTests : IProviderTest {
		private readonly HearThisAtProvider _provider;

		public HearThisAtTests() {
			var serviceCollection = new ServiceCollection()
			   .AddHttpClient()
			   .AddSingleton<HearThisAtProvider>();
			var provider = serviceCollection.BuildServiceProvider();
			_provider = provider.GetRequiredService<HearThisAtProvider>();
		}

		[DataTestMethod]
		[DataRow("Hard Bass")]
		[DataRow("Techo House")]
		[DataRow("Shawne")]
		[DataRow("Dubstep Mix")]
		[DataRow("Deep House")]
		public async Task SearchAsync(string query) {
			var trackResults = await _provider.SearchAsync(query)
			   .ConfigureAwait(false);

			Assert.IsNotNull(trackResults);
			Assert.IsTrue(trackResults.Count != 0);
			foreach (var track in trackResults) {
				Assert.IsNotNull(track);
				Assert.IsNotNull(track.Author);
				Assert.IsNotNull(track.Title);
				Assert.IsNotNull(track.StreamUrl);
				Assert.IsTrue(track.IsStreamable);
			}
		}

		[DataTestMethod]
		[DataRow("https://hearthis.at/momix-bond-r3/love-yourz-to-grrove-lp/listen")]
		[DataRow("https://hearthis.at/robert-k.-yw/robert-k-Bve/")]
		[DataRow("https://hearthis.at/rdio-gilo-4a/nao-faz-sentido-no-28/listen")]
		[DataRow("https://hearthis.at/groby-stefangrob/the-terminal-frontier-april-1992/listen")]
		[DataRow("https://hearthis.at/mjnrxbth/together-again-jazz-stacks/listen")]
		public async Task GetTrackAsync(string trackUrl) {
			var track = await _provider.GetTrackAsync(trackUrl)
			   .ConfigureAwait(false);

			Assert.IsNotNull(track);
			Assert.IsNotNull(track.Author);
			Assert.IsNotNull(track.Title);
			Assert.IsNotNull(track.StreamUrl);
			Assert.IsTrue(track.IsStreamable);
		}

		[DataTestMethod]
		[DataRow("https://hearthis.at/generik/dance-monkey-generik-remix/")]
		[DataRow("https://hearthis.at/8vlxvyrp/marshmello-anne-marie-friends-reggaeton-mix-bassworm/")]
		[DataRow("https://hearthis.at/djrocco/calmadjroccoremix/")]
		[DataRow("https://hearthis.at/cristiangil-dj/omar-montes-bad-gyal-alocao-cristian-gil-dj-extended-remix/")]
		[DataRow("https://hearthis.at/alessandro-caro/mau-y-ricky-camilo-lunay-la-boca-remix/")]
		public async Task GetPlaylistAsync(string playlistUrl) {
			var playlist = await _provider.GetPlaylistAsync(playlistUrl)
			   .ConfigureAwait(false);

			Assert.IsTrue(playlist.Tracks.Count > 0);
			Assert.IsNotNull(playlist.Id);
			Assert.IsNotNull(playlist.Author);
			Assert.IsNotNull(playlist.Tracks);
			Assert.IsNotNull(playlist.Url);

			foreach (var track in playlist.Tracks) {
				Assert.IsNotNull(track);
				Assert.IsNotNull(track.Author);
				Assert.IsNotNull(track.Title);
				Assert.IsNotNull(track.StreamUrl);
				Assert.IsTrue(track.IsStreamable);
			}
		}

		[DataTestMethod]
		[DataRow("https://hearthis.at/momix-bond-r3/love-yourz-to-grrove-lp/listen")]
		[DataRow("https://hearthis.at/robert-k.-yw/robert-k-Bve/")]
		[DataRow("https://hearthis.at/rdio-gilo-4a/nao-faz-sentido-no-28/listen")]
		[DataRow("https://hearthis.at/groby-stefangrob/the-terminal-frontier-april-1992/listen")]
		[DataRow("https://hearthis.at/mjnrxbth/together-again-jazz-stacks/listen")]
		public async Task GetStreamAsync(string trackUrl) {
		}
	}
}