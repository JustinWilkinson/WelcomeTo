using System.Threading.Tasks;

namespace WelcomeTo.Shared
{
    public interface IGameHub
    {
        Task AddToGroupAsync(string groupId);

        Task RemoveFromGroupAsync(string groupId);

        Task UpdateGameAsync(string gameId);

        Task SendGameMessageAsync(string gameId, GameMessage chatMessage);

        Task OtherPlayerActionTakenAsync(string gameId, string playerName);
    }
}