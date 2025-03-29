namespace NotUndeserved.Twitch.Widgets.ProgressBar.Application.interfaces {
    public interface IStreamElementsConfigService {
        string SocketUrl { get; }
        string AccessToken { get; }
        Task LoadConfigurationAsync();
    }
}
