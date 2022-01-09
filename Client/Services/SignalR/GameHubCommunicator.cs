using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using WelcomeTo.Shared.Interfaces;

namespace WelcomeTo.Client.Services.SignalR
{
    public class GameHubCommunicator : HubCommunicator, IGameHub
    {
        public GameHubCommunicator(NavigationManager navigationManager) : base("/GameHub", navigationManager)
        {

        }

        public Task AddToGroupAsync(string groupId) => _hubConnection.InvokeAsync("AddToGroupAsync", groupId);

        public Task RemoveFromGroupAsync(string groupId) => _hubConnection.InvokeAsync("RemoveFromGroupAsync", groupId);

        public Task UpdateGameAsync(string gameId) => _hubConnection.InvokeAsync("UpdateGameAsync", gameId);

        public Task OtherPlayerActionTakenAsync(string gameId, string playerName) => _hubConnection.InvokeAsync("OtherPlayerActionTakenAsync", gameId, playerName);

        public Task NewGameAddedAsync() => _hubConnection.InvokeAsync("NewGameAddedAsync");
    }
}