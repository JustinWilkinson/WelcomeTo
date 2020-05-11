using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using WelcomeTo.Server.Extensions;
using WelcomeTo.Server.Repository;
using WelcomeTo.Server.Services;
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
        private readonly IGameBuilder _gameBuilder;

        public GameController(ILogger<GameController> logger, IGameRepository gameRepository, IGameCountRepository gameCountRepository, IGameBuilder gameBuilder)
        {
            _logger = logger;
            _gameRepository = gameRepository;
            _gameCountRepository = gameCountRepository;
            _gameBuilder = gameBuilder;
        }

        [HttpPut("New")]
        public void New(JsonElement json)
        {
            _gameRepository.CreateGame(_gameBuilder.Build(json.GetStringProperty("GameId"), json.GetStringProperty("Name")), json.GetBooleanProperty("PrivateGame"));
            _gameCountRepository.IncrementGameCount();
        }

        [HttpPost("Start")]
        public void StartGame(JsonElement gameIdJson)
        {
            _gameRepository.ModifyGame(gameIdJson.GetString(), game =>
            {
                game.StartedAtUtc = DateTime.UtcNow;
                game.StartNextTurn();
            });
        }

        [HttpPost("NextTurn")]
        public void StartNextTurn(JsonElement gameIdJson) => _gameRepository.ModifyGame(gameIdJson.GetString(), game => game.StartNextTurn());

        [HttpPost("Join")]
        public Player Join(JsonElement gameIdJson)
        {
            return _gameRepository.ModifyGame(gameIdJson.GetString(), game =>
            {
                var player = new Player { Name = $"Player {game.Players.Count}", Board = _gameBuilder.StartingBoard, ScoreSheet = _gameBuilder.StartingScoreSheet };
                game.Players.Add(player);
                return player;
            });
        }

        [HttpPost("UpdatePlayerName")]
        public void UpdatePlayerName(JsonElement json)
        {
            _gameRepository.ModifyGame(json.GetStringProperty("GameId"), game => game.Players.Single(p => p.Name == json.GetStringProperty("OldName")).Name = json.GetStringProperty("NewName"));
        }

        [HttpPost("UpdatePlayerSheet")]
        public void UpdatePlayerSheet(JsonElement json)
        {
            _gameRepository.ModifyGame(json.GetStringProperty("GameId"), game =>
            {
                var newPlayerInfo = json.GetObjectProperty<Player>("Player");
                var dbPlayerInfo = game.Players.Single(p => p.Name == newPlayerInfo.Name);
                dbPlayerInfo.Board = newPlayerInfo.Board;
                dbPlayerInfo.ScoreSheet = newPlayerInfo.ScoreSheet;
            });
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