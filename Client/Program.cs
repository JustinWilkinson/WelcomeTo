using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Threading.Tasks;
using WelcomeTo.Client.Services;
using WelcomeTo.Client.Services.SignalR;

namespace WelcomeTo.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpService(builder.HostEnvironment.BaseAddress);
            builder.Services.AddGameStorage();
            builder.Services.AddBlazorTimer();
            builder.Services.AddHubCommunicator<GameHubCommunicator>();

            await builder.Build().RunAsync();
        }
    }
}