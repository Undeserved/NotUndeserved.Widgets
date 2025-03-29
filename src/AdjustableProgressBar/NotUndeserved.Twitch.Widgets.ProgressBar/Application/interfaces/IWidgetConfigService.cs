namespace NotUndeserved.Twitch.Widgets.ProgressBar.Application.interfaces {
    public interface IWidgetConfigService {
        int MaxValue { get; }
        int StepSize { get; }
        string BackgroundColour { get; }
        Task LoadConfigurationAsync();
    }
}
