using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using WelcomeTo.Shared;

namespace WelcomeTo.Client.Services.SignalR
{
    public class GameHubCommunicator : HubCommunicator, IGameHub
    {
        public GameHubCommunicator(NavigationManager navigationManager) : base("/GameHub", navigationManager)
        {

        }

        public async Task AddToGroupAsync(string groupId) => await _hubConnection.InvokeAsync("AddToGroupAsync", groupId);

        public async Task RemoveFromGroupAsync(string groupId) => await _hubConnection.InvokeAsync("RemoveFromGroupAsync", groupId);

        public async Task UpdateGameAsync(string gameId) => await _hubConnection.InvokeAsync("UpdateGameAsync", gameId);

        public async Task OtherPlayerActionTakenAsync(string gameId, string playerName) => await _hubConnection.InvokeAsync("OtherPlayerActionTakenAsync", gameId, playerName);

        public async Task NewGameAddedAsync() => await _hubConnection.InvokeAsync("NewGameAddedAsync");
    }
}