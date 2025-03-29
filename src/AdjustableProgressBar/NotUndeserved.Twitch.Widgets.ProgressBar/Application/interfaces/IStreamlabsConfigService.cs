namespace NotUndeserved.Twitch.Widgets.ProgressBar.Application.interfaces {
    public interface IStreamlabsConfigService {
        string SocketUrl { get; }
        string AccessToken { get; }
        Task LoadConfigurationAsync();
    }
}
