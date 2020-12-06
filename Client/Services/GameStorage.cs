using Cloudcrate.AspNetCore.Blazor.Browser.Storage;

namespace WelcomeTo.Client.Services
{
    public class GameStorage
    {
        private readonly LocalStorage _storage;

        public GameStorage(LocalStorage localStorage)
        {
            _storage = localStorage;
        }

        private string _gameId = null;
        public string GameId
        {
            get => _gameId ?? _storage.GetItem("GameId");
            set
            {
                _gameId = value;
                if (value is not null)
                {
                    _storage.SetItem("GameId", value);
                }
                else
                {
                    _storage.RemoveItem("GameId");
                }
            }
        }

        private string _playerName = null;
        public string PlayerName
        {
            get => _playerName ?? _storage.GetItem("PlayerName");
            set
            {
                _playerName = value;
                if (value is not null)
                {
                    _storage.SetItem("PlayerName", value);
                }
                else
                {
                    _storage.RemoveItem("PlayerName");
                }
            }
        }
    }
}