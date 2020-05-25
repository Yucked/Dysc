using System.Linq;
using System.Threading.Tasks;
using Dysc.Providers.SoundCloud;
using Dysc.Search;
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
        public async Task PerformSearchAsync(string query) {
            var response = await _provider.SearchAsync(query)
                .ConfigureAwait(false);

            Assert.AreEqual(SearchStatus.SearchResult, response.Status);
            Assert.IsNotNull(response.Tracks);
            Assert.IsTrue(response.Tracks.Count > 0);
            Assert.IsNull(response.Playlist.Name);
        }

        [DataTestMethod]
        [DataRow("https://soundcloud.com/kanyewest/sets/ye-49")]
        [DataRow("https://soundcloud.com/albsoon/sets/the-weeknd-more-balloons-remixed-by-sango")]
        [DataRow("https://soundcloud.com/jojiofficial/sets/ballads-1-3")]
        [DataRow("https://soundcloud.com/logic_official/sets/confessions-of-a-dangerous-3")]
        [DataRow("https://soundcloud.com/eminemofficial/sets/kamikaze-34")]
        public async Task GetPlaylistAsync(string playlistUrl) {
            var response = await _provider.SearchAsync(playlistUrl)
                .ConfigureAwait(false);

            Assert.AreEqual(SearchStatus.PlaylistLoaded, response.Status);
            Assert.IsNotNull(response.Playlist);
            Assert.IsNotNull(response.Playlist.Name);
            Assert.IsNotNull(response.Tracks);
            Assert.IsTrue(response.Tracks.Count > 0);
        }

        [DataTestMethod]
        [DataRow("https://soundcloud.com/theweeknd/hurt-you")]
        [DataRow("https://soundcloud.com/gesaffelstein/lost-in-the-fire")]
        [DataRow("https://soundcloud.com/kanyewest/i-love-it-freaky-girl-edit")]
        [DataRow("https://soundcloud.com/theweeknd/blinding-lights")]
        [DataRow("https://soundcloud.com/theweeknd/heartless")]
        public async Task GetTrackAsync(string trackUrl) {
            var response = await _provider.SearchAsync(trackUrl)
                .ConfigureAwait(false);

            Assert.AreEqual(SearchStatus.TrackLoaded, response.Status);
            Assert.IsTrue(response.Tracks.Count == 1);
        }

        [DataTestMethod]
        [DataRow("https://soundcloud.com/theweeknd/hurt-you")]
        [DataRow("https://soundcloud.com/gesaffelstein/lost-in-the-fire")]
        [DataRow("https://soundcloud.com/kanyewest/i-love-it-freaky-girl-edit")]
        [DataRow("https://soundcloud.com/theweeknd/blinding-lights")]
        [DataRow("https://soundcloud.com/theweeknd/heartless")]
        public async Task GetStreamAsync(string trackUrl) {
            var search = await _provider
                .SearchAsync(trackUrl)
                .ConfigureAwait(false);

            var track = search.Tracks.FirstOrDefault();
            Assert.IsNotNull(track);

            var stream = await _provider.GetStreamAsync(track)
                .ConfigureAwait(false);

            Assert.IsNotNull(stream);
            Assert.IsFalse(stream.Length == 0);
        }
    }
}