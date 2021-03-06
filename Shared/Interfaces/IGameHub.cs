﻿using System.Threading.Tasks;

namespace WelcomeTo.Shared.Interfaces
{
    public interface IGameHub
    {
        Task AddToGroupAsync(string groupId);

        Task RemoveFromGroupAsync(string groupId);

        Task UpdateGameAsync(string gameId);

        Task OtherPlayerActionTakenAsync(string gameId, string playerName);

        Task NewGameAddedAsync();
    }
}