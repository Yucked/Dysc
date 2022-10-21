using System.Threading.Tasks;

namespace Dysc.Tests.Interfaces {
	public interface IProviderTest {
		Task SearchAsync(string query);

		Task GetTrackAsync(string trackUrl);

		Task GetPlaylistAsync(string playlistUrl);

		Task GetStreamAsync(string trackUrl);
	}
}