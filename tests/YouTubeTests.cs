using System.Linq;
using System.Threading.Tasks;
using Dysc.Providers.YouTube;
using Dysc.Search;
using Dysc.Tests.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dysc.Tests {
    [TestClass]
    public sealed class YouTubeTests : IProviderTest {
        private readonly YouTubeProvider _provider;

        public YouTubeTests() {
            var serviceCollection = new ServiceCollection()
                .AddHttpClient()
                .AddSingleton<YouTubeProvider>()
                .AddSingleton<YouTubeParser>();

            var provider = serviceCollection.BuildServiceProvider();
            _provider = provider.GetRequiredService<YouTubeProvider>();
        }

        [DataTestMethod]
        [DataRow("SiR Hair Down")]
        [DataRow("J. Cole No Role Modelz")]
        [DataRow("The Weeknd Call Out My Name")]
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
        [DataRow("https://www.youtube.com/watch?v=ZnKvQbpDYXU&list=PLVfin74Qx3tXoklqU30Xqf8nk9qbgNM8Y")]
        [DataRow("https://www.youtube.com/playlist?list=PL8Go-YHXcY4bnSo0BDjYt9KuW9izuBD8U")]
        [DataRow("https://www.youtube.com/playlist?list=OLAK5uy_nrbHEhhkZIpj3-XdSQm75NdaqxRScGpQc&playnext=1&index=1")]
        [DataRow("https://www.youtube.com/watch?v=-Bg_cVW7vcQ&list=PLTuEWbP7pCjCgg1ET9P6h-Iipre6fQ7sg")]
        [DataRow("https://www.youtube.com/watch?v=uD4izuDMUQA&list=PLWS2mFp_C6rPPIPDdjOqPIM3b-yxxcFmu")]
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
        [DataRow("https://www.youtube.com/watch?v=JqvdAuuXVyA")]
        [DataRow("https://www.youtube.com/watch?v=aaet7EygdsE")]
        [DataRow("https://www.youtube.com/watch?v=pERXVqS54XE")]
        [DataRow("https://www.youtube.com/watch?v=ZnKvQbpDYXU")]
        [DataRow("https://www.youtube.com/watch?v=BaOScwq_lZs")]
        [DataRow("http://www.youtube.com/watch?v=-wtIMTCHWuI")]
        [DataRow("http://youtu.be/-wtIMTCHWuI")]
        [DataRow("https: //www.youtube.com/watch?v=0zM3nApSvMg#t=0m10s")]
        [DataRow("http://www.youtube.com/?v=dQw4w9WgXcQ")]
        [DataRow("http://youtu.be/oTJRivZTMLs&feature=channel")]
        [DataRow("https://www.youtube.com/watch?v=JqvdAuuXVyA")]
        [DataRow("https://www.youtube.com/watch?v=aaet7EygdsE")]
        public async Task GetTrackAsync(string trackUrl) {
            var response = await _provider.SearchAsync(trackUrl)
                .ConfigureAwait(false);

            var track = response.Tracks.FirstOrDefault();
            YouTubeParser.ParseId(trackUrl, out var videoId, out _);

            Assert.AreEqual(SearchStatus.TrackLoaded, response.Status);
            Assert.IsTrue(response.Tracks.Count == 1, $"{response.Tracks.Count} tracks.");
            Assert.IsTrue(track.Id == videoId, $"{track.Id} != {videoId}");
        }

        [DataTestMethod]
        [DataRow("https://www.youtube.com/watch?v=JqvdAuuXVyA")]
        [DataRow("https://www.youtube.com/watch?v=aaet7EygdsE")]
        [DataRow("https://www.youtube.com/watch?v=pERXVqS54XE")]
        [DataRow("https://www.youtube.com/watch?v=ZnKvQbpDYXU")]
        [DataRow("https://www.youtube.com/watch?v=BaOScwq_lZs")]
        public async Task GetStreamAsync(string trackUrl) {
            var stream = await _provider.GetStreamAsync(trackUrl)
                .ConfigureAwait(false);

            Assert.IsNotNull(stream);
            Assert.IsTrue(stream.CanRead);
            Assert.IsFalse(stream.Length == 0);
        }

        [DataTestMethod]
        [DataRow("http://www.youtube.com/watch?v=-wtIMTCHWuI")]
        [DataRow("http://youtu.be/-wtIMTCHWuI")]
        [DataRow("https: //www.youtube.com/watch?v=0zM3nApSvMg#t=0m10s")]
        [DataRow("http://www.youtube.com/?v=dQw4w9WgXcQ")]
        [DataRow("http://youtu.be/oTJRivZTMLs&feature=channel")]
        [DataRow("https://www.youtube.com/watch?v=JqvdAuuXVyA")]
        [DataRow("https://www.youtube.com/watch?v=aaet7EygdsE")]
        public void ParseVideoId(string url) {
            YouTubeParser.ParseId(url, out var videoId, out _);
            Assert.IsNotNull(videoId);
        }

        [DataTestMethod]
        [DataRow("https://www.youtube.com/watch?v=ZnKvQbpDYXU&list=PLVfin74Qx3tXoklqU30Xqf8nk9qbgNM8Y")]
        [DataRow("https://www.youtube.com/playlist?list=PL8Go-YHXcY4bnSo0BDjYt9KuW9izuBD8U")]
        [DataRow("https://www.youtube.com/playlist?list=OLAK5uy_nrbHEhhkZIpj3-XdSQm75NdaqxRScGpQc&playnext=1&index=1")]
        [DataRow("https://www.youtube.com/watch?v=-Bg_cVW7vcQ&list=PLTuEWbP7pCjCgg1ET9P6h-Iipre6fQ7sg")]
        [DataRow("https://www.youtube.com/watch?v=uD4izuDMUQA&list=PLWS2mFp_C6rPPIPDdjOqPIM3b-yxxcFmu")]
        public void ParsePlaylistId(string url) {
            YouTubeParser.ParseId(url, out _, out var playlistId);
            Assert.IsNotNull(playlistId);
        }
    }
}