using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using WelcomeTo.Server.Extensions;
using WelcomeTo.Shared.Abstractions;
using WelcomeTo.Shared.Extensions;

namespace WelcomeTo.Server.Repository
{
    public interface IGameRepository
    {
        void CreateGame(Game game, bool privateGame);

        Game GetGame(string id);

        IEnumerable<Game> ListGames(bool includePrivate = false);

        void Save(Game game);

        void ModifyGame(string gameId, Action<Game> action);

        T ModifyGame<T>(string gameId, Func<Game, T> function);

        void DeleteGames(IEnumerable<Guid> gameIds);
    }

    public class GameRepository : Repository, IGameRepository
    {
        private readonly ILogger<GameRepository> _logger;

        public GameRepository(ILogger<GameRepository> logger) : base("CREATE TABLE IF NOT EXISTS Games (Id text PRIMARY KEY, GameJson text NOT NULL, Private integer NOT NULL CHECK (Private IN (0,1)))")
        {
            _logger = logger;
        }

        public void CreateGame(Game game, bool privateGame)
        {
            try
            {
                var command = new SQLiteCommand("INSERT INTO Games (Id, GameJson, Private) VALUES(@Id, @Json, @Private)");
                command.AddParameter("@Id", game.Id);
                command.AddParameter("@Json", game.Serialize());
                command.AddParameter("@Private", privateGame ? 1 : 0);
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating a new game.");
                throw;
            }
        }


        public Game GetGame(string id)
        {
            try
            {
                var command = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id");
                command.AddParameter("@Id", id);
                return Execute(command, DeserializeColumn<Game>("GameJson")).SingleOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred retrieving game '{id}'.");
                throw;
            }
        }


        public IEnumerable<Game> ListGames(bool includePrivate = false)
        {
            try
            {
                return Execute($"SELECT * FROM Games{(includePrivate ? "" : " WHERE Private = 0")}", DeserializeColumn<Game>("GameJson"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred listing games.");
                throw;
            }
        }

        public void Save(Game game)
        {
            try
            {
                var command = new SQLiteCommand("UPDATE Games SET GameJson = @Json WHERE Id = @Id");
                command.AddParameter("@Id", game.Id);
                command.AddParameter("@Json", game.Serialize());
                Execute(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred saving game '{game.Id}'.");
                throw;
            }
        }

        public void ModifyGame(string gameId, Action<Game> action)
        {
            ExecuteInTransaction(connection =>
            {
                var selectCommand = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id;", connection);
                selectCommand.AddParameter("@Id", gameId);
                using var reader = selectCommand.ExecuteReader();
                if (reader.Read())
                {
                    var game = DeserializeColumn<Game>("GameJson")(reader);
                    action(game);
                    UpdateGame(connection, game);
                }
            });
        }

        public T ModifyGame<T>(string gameId, Func<Game, T> function)
        {
            T returnValue = default;

            ExecuteInTransaction(connection =>
            {
                var selectCommand = new SQLiteCommand("SELECT GameJson FROM Games WHERE Id = @Id;", connection);
                selectCommand.AddParameter("@Id", gameId);
                using var reader = selectCommand.ExecuteReader();
                if (reader.Read())
                {
                    var game = DeserializeColumn<Game>("GameJson")(reader);
                    returnValue = function(game);
                    UpdateGame(connection, game);
                }
            });

            return returnValue;
        }

        public void DeleteGames(IEnumerable<Guid> gameIds)
        {
            try
            {
                ExecuteInTransaction((connection) =>
                {
                    foreach (var gameId in gameIds)
                    {
                        var command = new SQLiteCommand("DELETE FROM Games WHERE Id = @Id", connection);
                        command.AddParameter("@Id", gameId);
                        command.ExecuteNonQuery();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred clearing old games.");
                throw;
            }
        }

        private void UpdateGame(SQLiteConnection connection, Game game)
        {
            var updateCommand = new SQLiteCommand("UPDATE Games SET GameJson = @Json WHERE Id = @Id", connection);
            updateCommand.AddParameter("@Id", game.Id);
            updateCommand.AddParameter("@Json", game.Serialize());
            updateCommand.ExecuteNonQuery();
        }
    }
}