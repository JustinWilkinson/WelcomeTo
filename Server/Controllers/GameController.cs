using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WelcomeTo.Server.Repository;
using WelcomeTo.Server.Services;
using WelcomeTo.Shared.Abstractions;
using WelcomeTo.Shared.Extensions;

namespace WelcomeTo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly IGameRepository _gameRepository;
        private readonly IGameCountRepository _gameCountRepository;
        private readonly IRecordRepository _recordRepository;
        private readonly IGameBuilder _gameBuilder;

        public GameController(ILogger<GameController> logger, IGameRepository gameRepository, IGameCountRepository gameCountRepository, IRecordRepository recordRepository, IGameBuilder gameBuilder)
        {
            _logger = logger;
            _gameRepository = gameRepository;
            _gameCountRepository = gameCountRepository;
            _recordRepository = recordRepository;
            _gameBuilder = gameBuilder;
        }

        [HttpPut("New")]
        public Task New(JsonElement json)
        {
            return Task.WhenAll(
                _gameRepository.CreateGame(_gameBuilder.Build(json.GetStringProperty("GameId"), json.GetStringProperty("Name")), json.GetBooleanProperty("PrivateGame")),
                _gameCountRepository.IncrementGameCount());
        }

        [HttpPost("Start")]
        public Task StartGame(JsonElement gameIdJson)
        {
            return _gameRepository.ModifyGame(gameIdJson.GetString(), game =>
            {
                game.StartedAtUtc = DateTime.UtcNow;
                game.StartNextTurn();
            });
        }

        [HttpPost("Join")]
        public Task<string> Join(JsonElement gameIdJson)
        {
            return _gameRepository.ModifyGame(gameIdJson.GetString(), game =>
            {
                var playerCount = game.Players.Count;
                var isHost = playerCount == 0;
                var player = new Player
                {
                    Name = isHost ? "Host" : $"Guest {playerCount}",
                    IsHost = isHost,
                    Board = _gameBuilder.StartingBoard,
                    ScoreSheet = _gameBuilder.StartingScoreSheet
                };
                game.Players.Add(player);
                return player.Serialize();
            });
        }

        [HttpPost("UpdatePlayerName")]
        public Task<bool> UpdatePlayerName(JsonElement json)
        {
            return _gameRepository.ModifyGame(json.GetStringProperty("GameId"), game => 
            {
                var newName = json.GetStringProperty("NewName");
                var canChangeName = !game.Players.Any(x => x.Name == newName);
                if (canChangeName)
                {
                    game.Players.Single(p => p.Name == json.GetStringProperty("OldName")).Name = newName;
                }

                return canChangeName;
            });
        }

        [HttpPost("UpdatePlayerSheet")]
        public async Task<string> UpdatePlayerSheet(JsonElement json)
        {
            var game = await _gameRepository.ModifyGame(json.GetStringProperty("GameId"), game =>
            {
                var newPlayerInfo = json.GetObjectProperty<Player>("Player");
                var player = game.Players.Single(p => p.Name == newPlayerInfo.Name);
                player.Board = newPlayerInfo.Board;
                player.ScoreSheet = newPlayerInfo.ScoreSheet;
                player.HideBoard = newPlayerInfo.HideBoard;
                
                game.CurrentTurn.PlayerNamesWithActionTaken.Add(player.Name);

                if (!game.Players.Select(x => x.Name).Except(game.CurrentTurn.PlayerNamesWithActionTaken).Any())
                {
                    game.StartNextTurn();
                }

                return game;
            });

            if (game.CompletedAtUtc.HasValue)
            {
                await _recordRepository.UpdateRecords(game);
            }

            return game.Serialize();
        }

        [HttpPost("RequestReshuffle")]
        public Task RequestReshuffle(JsonElement json) 
            => _gameRepository.ModifyGame(json.GetStringProperty("GameId"), game => game.CurrentTurn.ReshuffleRequesters.Add(json.GetStringProperty("RequesterName")));

        [HttpGet("Get")]
        public Task<string> Get(string id) => _gameRepository.GetGame(id).SerializeAsync();

        [HttpGet("List")]
        public Task<string> List() => _gameRepository.ListGames().SerializeAsync();

        [HttpGet("Count")]
        public Task<int> Get() => _gameCountRepository.GetGameCount();

        [HttpPost("RemovePlayer")]
        public Task<string> RemovePlayer(JsonElement json)
        {
            return _gameRepository.ModifyGame(json.GetStringProperty("GameId"), game =>
            {
                game.Players.RemoveAt(game.Players.FindIndex(p => p.Name == json.GetStringProperty("KickedPlayerName")));
                return game.Serialize();
            });
        }
    }
}