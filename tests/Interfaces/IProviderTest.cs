using System.Threading.Tasks;

namespace Dysc.Tests.Interfaces {
    public interface IProviderTest {
        Task PerformSearchAsync(string query);

        Task GetPlaylistAsync(string playlistUrl);

        Task GetTrackAsync(string trackUrl);

        Task GetStreamAsync(string trackUrl);
    }
}