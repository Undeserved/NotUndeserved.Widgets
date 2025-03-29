using NotUndeserved.Twitch.Widgets.ProgressBar.Application.common;
using NotUndeserved.Twitch.Widgets.ProgressBar.Application.interfaces;
using System.Diagnostics;
using System.Net.Http.Json;

namespace NotUndeserved.Twitch.Widgets.ProgressBar.Services {
    public class StreamlabsConfigService : IStreamlabsConfigService {
        private readonly HttpClient _httpClient;
        public string SocketUrl { get; private set; } = "wss://sockets.streamlabs.com/socket.io/?EIO=3&transport=websocket";
        public string AccessToken { get; private set; } = string.Empty;

        public StreamlabsConfigService(HttpClient httpClient) {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task LoadConfigurationAsync() {
            var config = await _httpClient.GetFromJsonAsync<StreamlabsConfig>("config.json");
            if (config != null) {
                SocketUrl = config.SocketUrl;
                AccessToken = config.AccessToken;
            } else {
                Debug.WriteLine("Streamlabs config data is null. Using default values.");
            }
        }
    }
}
