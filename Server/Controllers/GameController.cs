using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;
using WelcomeTo.Server.Extensions;
using WelcomeTo.Server.Repository;
using WelcomeTo.Shared;

namespace WelcomeTo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly IGameRepository _gameRepository;
        private readonly IGameCountRepository _gameCountRepository;

        public GameController(ILogger<GameController> logger, IGameRepository gameRepository, IGameCountRepository gameCountRepository)
        {
            _logger = logger;
            _gameRepository = gameRepository;
            _gameCountRepository = gameCountRepository;
        }

        [HttpPut("New")]
        public void New(JsonElement json)
        {
            _gameRepository.CreateGame(Game.NewGame(json.GetStringProperty("GameId"), json.GetStringProperty("Name")), json.GetBooleanProperty("PrivateGame"));
            _gameCountRepository.IncrementGameCount();
        }

        [HttpPost("Start")]
        public void StartGame(JsonElement gameIdJson) => _gameRepository.ModifyGame(gameIdJson.GetString(), game => game.StartGame());

        [HttpPost("Join")]
        public Player Join(JsonElement gameIdJson)
        {
            return _gameRepository.ModifyGame(gameIdJson.GetString(), game =>
            {
                var player = new Player { Name = $"Player {game.Players.Count}" };
                game.Players.Add(player);
                return player;
            });
        }

        [HttpPost("UpdatePlayer")]
        public void UpdatePlayer(JsonElement json)
        {
            _gameRepository.ModifyGame(json.GetStringProperty("GameId"), game => game.Players.Single(p => p.Name == json.GetStringProperty("OldName")).Name = json.GetStringProperty("NewName"));
        }

        [HttpPost("Save")]
        public void Save(JsonElement json) => _gameRepository.Save(json.Deserialize<Game>());

        [HttpGet("Get")]
        public string Get(string id) => _gameRepository.GetGame(id).Serialize();

        [HttpGet("List")]
        public string List() => _gameRepository.ListGames().Serialize();

        [HttpGet("Count")]
        public int Get() => _gameCountRepository.GetGameCount();
    }
}