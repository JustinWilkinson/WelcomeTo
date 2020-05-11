using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WelcomeTo.Shared;

namespace WelcomeTo.Server.Hubs
{
    public class GameHub : Hub, IGameHub
    {
        public async Task AddToGroupAsync(string groupId) => await Groups.AddToGroupAsync(Context.ConnectionId, groupId);

        public async Task RemoveFromGroupAsync(string groupId) => await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);

        public async Task UpdateGameAsync(string gameId) => await Clients.OthersInGroup(gameId).SendAsync("UpdateGame");

        public async Task SendGameMessageAsync(string gameId, GameMessage chatMessage) => await Clients.OthersInGroup(gameId).SendAsync(gameId, chatMessage);
    }
}