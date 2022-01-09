using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using WelcomeTo.Server.Extensions;
using WelcomeTo.Shared.Abstractions;
using WelcomeTo.Shared.Extensions;

namespace WelcomeTo.Server.Repository
{
    public interface IGameRepository
    {
        Task CreateGame(Game game, bool privateGame);

        Task<Game> GetGame(string id);

        IAsyncEnumerable<Game> ListGames(bool includePrivate = false);

        Task Save(Game game);

        Task ModifyGame(string gameId, Action<Game> action);

        Task<T> ModifyGame<T>(string gameId, Func<Game, T> function);

        Task DeleteGames(IEnumerable<Guid> gameIds);
    }

    public class GameRepository : Repository, IGameRepository
    {
        private readonly ILogger<GameRepository> _logger;

        public GameRepository(ILogger<GameRepository> logger)
        {
            _logger = logger;
        }

        protected override Task InitializeAsync() => Execute("CREATE TABLE IF NOT EXISTS Games (Id text PRIMARY KEY, GameJson text NOT NULL, Private integer NOT NULL CHECK (Private IN (0,1)))");

        public async Task CreateGame(Game game, bool privateGame)
        {
            try
            {
                await using var command = new SQLiteCommand("INSERT INTO Games (Id, GameJson, Private) VALUES(@Id, @Json, @Private)")
                    .AddParameter("@Id", game.Id)
                    .AddParameter("@Json", game.Serialize())
                    .AddParameter("@Private", privateGame ? 1 : 0);

                await Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating a new game.");
                throw;
            }
        }

        public async Task<Game> GetGame(string id)
        {
            try
            {
                var command = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id").AddParameter("@Id", id);
                return await Execute(command, DeserializeColumn<Game>("GameJson")).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred retrieving game '{id}'.", id);
                throw;
            }
        }

        public IAsyncEnumerable<Game> ListGames(bool includePrivate = false)
            => Execute($"SELECT * FROM Games{(includePrivate ? "" : " WHERE Private = 0")}", DeserializeColumn<Game>("GameJson"))
                .WithCatch(ex => _logger.LogError(ex, "An error occurred listing games."));

        public async Task Save(Game game)
        {
            try
            {
                await using var command = new SQLiteCommand("UPDATE Games SET GameJson = @Json WHERE Id = @Id")
                    .AddParameter("@Id", game.Id)
                    .AddParameter("@Json", game.Serialize());

                await Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred saving game '{id}'.", game.Id);
                throw;
            }
        }

        public Task ModifyGame(string gameId, Action<Game> action)
        {
            return ExecuteInTransaction(async connection =>
            {
                await using var selectCommand = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id;", connection).AddParameter("@Id", gameId);
                using var reader = selectCommand.ExecuteReader();
                if (await reader.ReadAsync())
                {
                    var game = DeserializeColumn<Game>("GameJson")(reader);
                    action(game);
                    await UpdateGame(connection, game);
                }
            });
        }

        public async Task<T> ModifyGame<T>(string gameId, Func<Game, T> function)
        {
            T returnValue = default;

            await ExecuteInTransaction(async connection =>
            {
                await using var selectCommand = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id;", connection).AddParameter("@Id", gameId);
                await using var reader = selectCommand.ExecuteReader();
                if (reader.Read())
                {
                    var game = DeserializeColumn<Game>("GameJson")(reader);
                    returnValue = function(game);
                    await UpdateGame(connection, game);
                }
            });

            return returnValue;
        }

        public async Task DeleteGames(IEnumerable<Guid> gameIds)
        {
            try
            {
                await ExecuteInTransaction(async connection =>
                {
                    foreach (var gameId in gameIds)
                    {
                        await using var command = new SQLiteCommand("DELETE FROM Games WHERE Id = @Id", connection).AddParameter("@Id", gameId);
                        await command.ExecuteNonQueryAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred clearing old games.");
                throw;
            }
        }

        private static async Task UpdateGame(SQLiteConnection connection, Game game)
        {
            var updateCommand = new SQLiteCommand("UPDATE Games SET GameJson = @Json WHERE Id = @Id", connection)
                .AddParameter("@Id", game.Id)
                .AddParameter("@Json", game.Serialize());
            
            await updateCommand.ExecuteNonQueryAsync();
        }
    }
}