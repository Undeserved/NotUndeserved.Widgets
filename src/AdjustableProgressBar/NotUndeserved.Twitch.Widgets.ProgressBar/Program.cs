using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NotUndeserved.Twitch.Widgets.ProgressBar;
using NotUndeserved.Twitch.Widgets.ProgressBar.Application.interfaces;
using NotUndeserved.Twitch.Widgets.ProgressBar.Services;
using NotUndeserved.Twitch.Widgets.ProgressBar.UI.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddLogging(
    logging => {
        {
            logging.SetMinimumLevel(LogLevel.Debug);
        }
    });
builder.RootComponents.Add<App>("#app");

//Widget config
builder.Services.AddHttpClient<IWidgetConfigService, WidgetConfigService>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});
//Streamlabs config
builder.Services.AddHttpClient<IStreamElementsConfigService, StreamElementsConfigService>(client => {
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});
builder.Services.AddSingleton<ProgressState>();

var host = builder.Build();

var widgetConfigService = host.Services.GetRequiredService<IWidgetConfigService>();
await widgetConfigService.LoadConfigurationAsync();
var streamlabsConfigService = host.Services.GetRequiredService<IStreamElementsConfigService>();
await streamlabsConfigService.LoadConfigurationAsync();

await host.RunAsync();
