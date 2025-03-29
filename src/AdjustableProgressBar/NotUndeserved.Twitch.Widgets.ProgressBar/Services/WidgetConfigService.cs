using NotUndeserved.Twitch.Widgets.ProgressBar.Application.common;
using NotUndeserved.Twitch.Widgets.ProgressBar.Application.interfaces;
using System.Diagnostics;
using System.Net.Http.Json;

namespace NotUndeserved.Twitch.Widgets.ProgressBar.Services {
    public class WidgetConfigService : IWidgetConfigService {
        private readonly HttpClient _httpClient;
        public int MaxValue { get; private set; } = 100;
        public int StepSize { get; private set; } = 20;
        public string BackgroundColour { get; private set; } = "rgba(0, 255, 0, 0.7)";

        public WidgetConfigService(HttpClient httpClient) {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task LoadConfigurationAsync() {
            var config = await _httpClient.GetFromJsonAsync<WidgetConfig>("config.json");
            if (config != null) {
                MaxValue = config.MaxValue;
                StepSize = config.StepSize;
                BackgroundColour = config.BackgroundColour;
            } else {
                Debug.WriteLine("Widget config data is null. Using default values.");
            }
        }
    }
}
