using NotUndeserved.Twitch.Widgets.ProgressBar.Application.interfaces;
using NotUndeserved.Twitch.Widgets.ProgressBar.UI.State;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace NotUndeserved.Twitch.Widgets.ProgressBar.Infrastructure.Streamlabs {
    public class StreamElementsListener {
        private readonly ClientWebSocket _webSocket;
        private readonly ILogger<StreamElementsListener> _logger;
        private readonly ProgressState _progressState;
        private readonly string _socketUrl;
        private readonly string _accessToken;
        private CancellationTokenSource? _cts;

        public StreamElementsListener(IStreamElementsConfigService streamlabsConfigService, ProgressState progressState, ILogger<StreamElementsListener> logger) {
            _webSocket = new ClientWebSocket();
            _logger = logger;
            _progressState = progressState;
            _socketUrl = streamlabsConfigService.SocketUrl;
            _accessToken = streamlabsConfigService.AccessToken;
        }

        public async Task StartListeningAsync() {
            _cts = new CancellationTokenSource();
            await _webSocket.ConnectAsync(new Uri($"{_socketUrl}?token={_accessToken}"), _cts.Token);

            // Start listening
            _ = Task.Run(async () => await ListenAsync(), _cts.Token);
        }

        private async Task ListenAsync() {
            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open) {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text) {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    HandleMessage(message);
                }
            }
        }

        private void HandleMessage(string message) {
            try {
                var json = JsonDocument.Parse(message).RootElement;
                var eventType = json.GetProperty("type").GetString();
                var eventData = json.GetProperty("message");

                if (eventType == "event") {
                    var eventName = eventData[0].GetProperty("name").GetString();
                    _logger.LogInformation($"Received event: {eventName}");

                    if (eventName == "progress_increase") {
                        _progressState.IncreaseProgress();
                    } else if (eventName == "progress_reset") {
                        _progressState.ResetProgress();
                    }
                }
            } catch (Exception ex) {
                _logger.LogError($"Error processing Streamlabs event: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync() {
            _cts?.Cancel();
            _cts?.Dispose();
            if (_webSocket.State == WebSocketState.Open) {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
            _webSocket.Dispose();
        }

        //public async Task ConnectAsync() {
        //    await _webSocket.ConnectAsync(new Uri(_socketUrl), CancellationToken.None);
        //    await SendMessageAsync($"42[\"authenticate\", {{\"token\": \"{_accessToken}\"}}]");

        //    await ReceiveMessagesAsync();
        //}
        //private async Task SendMessageAsync(string message) {
        //    var bytes = Encoding.UTF8.GetBytes(message);
        //    await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        //}

        //private async Task ReceiveMessagesAsync() {
        //    var buffer = new byte[4096];
        //    while (_webSocket.State == WebSocketState.Open) {
        //        var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //        if (result.MessageType == WebSocketMessageType.Close)
        //            break;

        //        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        //        OnMessageReceived?.Invoke(message);
        //    }
        //}
    }
}
