using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using WelcomeTo.Server.Extensions;
using WelcomeTo.Shared;
using WelcomeTo.Shared.Enumerations;
using WelcomeTo.Shared.Extensions;

namespace WelcomeTo.Server.Repository
{
    /// <summary>
    /// Interface for managing the GameCount table.
    /// </summary>
    public interface IRecordRepository
    {
        IEnumerable<Record> ListRecords();

        void UpdateRecords(Game records);
    }

    /// <summary>
    /// Manages the GameCount table.
    /// </summary>
    public class RecordRepository : Repository, IRecordRepository
    {
        private const int RECORD_COUNT = 5;

        private readonly ILogger<RecordRepository> _logger;
        private readonly object _cacheLock =  new object();

        private List<Record> _fame = new List<Record>();
        private List<Record> _shame = new List<Record>();
        private bool _unsavedChanges = false;

        public RecordRepository(ILogger<RecordRepository> logger) : base("CREATE TABLE IF NOT EXISTS Records (RecordsJson text)")
        {
            _logger = logger;

            try
            {
                lock (_cacheLock)
                {
                    if (ExecuteScalar("SELECT COUNT(*) AS Count FROM Records", Convert.ToInt32) == 0)
                    {
                        var now = DateTime.UtcNow;
                        for (var i = 1; i <= RECORD_COUNT; i++)
                        {
                            _fame.Add(new Record { Type = RecordType.Fame, Position = i, Date = now });
                            _shame.Add(new Record { Type = RecordType.Shame, Position = i, Date = now });
                        }
                        UpdateRecords();
                    }
                    else
                    {
                        Execute("SELECT * FROM Records", DeserializeColumn<IEnumerable<Record>>("RecordsJson")).Single().ForEach(r => (r.Type == RecordType.Fame ? _fame : _shame).Add(r));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred initialising the Game Count Repository");
                throw;
            }

        }

        public IEnumerable<Record> ListRecords()
        {
            try
            {
                lock (_cacheLock)
                {
                    OrderRecords();
                    return _fame.Concat(_shame);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred retrieving records.");
                throw;
            }
        }

        public void UpdateRecords(Game game)
        {
            try
            {
                lock (_cacheLock)
                {
                    var now = DateTime.UtcNow;
                    foreach (var player in game.Players)
                    {
                        var score = game.GetPointsTotal(player);
                        if (CheckForFameRecord(game, player, score, now) || CheckForShameRecord(game, player, score, now))
                        {
                            _unsavedChanges = true;
                            OrderRecords();
                        }
                    }

                    if (_unsavedChanges)
                    {
                        UpdateRecords();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred updating the records.");
                throw;
            }
        }

        #region Private
        private void UpdateRecords()
        {
            try
            {
                var command = new SQLiteCommand("UPDATE Records SET RecordsJson = @RecordsJson");
                command.AddParameter("@RecordsJson", _fame.Concat(_shame).Serialize());
                Execute(command);
                _unsavedChanges = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred updating the records.");
                throw;
            }
        }

        private bool CheckForFameRecord(Game game, Player player, int score, DateTime now)
        {
            for (var index = 0; index < _fame.Count; index++)
            {
                var record = _fame[index];
                if (score >= record.Score)
                {
                    _fame.Add(new Record
                    {
                        Type = RecordType.Shame,
                        Game = game.Name,
                        Player = player.Name,
                        Score = score,
                        Date = now,
                        Position = record.Position
                    });

                    if (score != record.Score)
                    {
                        for (var incrementIndex = index + 1; incrementIndex < _fame.Count; incrementIndex++)
                        {
                            if (++_fame[incrementIndex].Position > RECORD_COUNT)
                            {
                                _fame.RemoveAt(incrementIndex);
                            }
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private bool CheckForShameRecord(Game game, Player player, int score, DateTime now)
        {
            for (var index = 0; index < _shame.Count; index++)
            {
                var record = _shame[index];
                if (score <= record.Score)
                {
                    _shame.Add(new Record
                    {
                        Type = RecordType.Shame,
                        Game = game.Name,
                        Player = player.Name,
                        Score = score,
                        Date = now,
                        Position = record.Position
                    });

                    if (score != record.Score)
                    {
                        for (var incrementIndex = index + 1; incrementIndex < _shame.Count; incrementIndex++)
                        {
                            if (++_shame[incrementIndex].Position > RECORD_COUNT)
                            {
                                _shame.RemoveAt(incrementIndex);
                            }
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private void OrderRecords()
        {
            _fame = _fame.OrderBy(x => x.Position).ThenBy(x => x.Player).ThenBy(x => x.Date).ToList();
            _shame = _shame.OrderBy(x => x.Position).ThenBy(x => x.Player).ThenBy(x => x.Date).ToList();
        }
        #endregion
    }
}