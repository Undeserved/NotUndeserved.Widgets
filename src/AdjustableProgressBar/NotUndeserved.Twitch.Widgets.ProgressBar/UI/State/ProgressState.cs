using NotUndeserved.Twitch.Widgets.ProgressBar.Application.interfaces;
using NotUndeserved.Twitch.Widgets.ProgressBar.Services;

namespace NotUndeserved.Twitch.Widgets.ProgressBar.UI.State {
    public class ProgressState {
        public event Action? OnProgressUpdated;

        private IWidgetConfigService _configService;
        public int MaxValue => _configService.MaxValue;
        public int Step => _configService.StepSize;
        public string BackgroundColour => _configService.BackgroundColour;

        private const int MinValue = 0;

        private int _fillPercentage = 0;
        public int FillPercentage {
            get => _fillPercentage;
            private set {
                _fillPercentage = Math.Min(value, MaxValue);
                OnProgressUpdated?.Invoke();
            }
        }

        public ProgressState(IWidgetConfigService configService) { 
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        public void IncreaseProgress() {
            FillPercentage += Step;
        }

        public void SetProgress(int TargetValue) {
            FillPercentage = TargetValue;
        }

        public void ResetProgress() {
            FillPercentage = MinValue;
        }
    }
}
