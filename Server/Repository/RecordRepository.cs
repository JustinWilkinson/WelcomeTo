using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using WelcomeTo.Server.Extensions;
using WelcomeTo.Shared.Abstractions;
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
                        Execute("INSERT INTO Records VALUES ('')");
                    }
                    else
                    {
                        Execute("SELECT * FROM Records", DeserializeColumn<IEnumerable<Record>>("RecordsJson")).SingleOrDefault()?.ForEach(r => (r.Type == RecordType.Fame ? _fame : _shame).Add(r));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred initialising the Record Repository");
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
                        if (CheckForFameRecord(game.Name, player.Name, score, now) | CheckForShameRecord(game.Name, player.Name, score, now)) // Use bitwise OR, want to always check both.
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

        private bool CheckForFameRecord(string gameName, string playerName, int score, DateTime now)
        {
            var beatenRecord = _fame.FirstOrDefault(r => score >= r.Score);
            if (beatenRecord is not null || _fame.Count < RECORD_COUNT)
            {
                var newRecord = new Record
                {
                    Type = RecordType.Fame,
                    Game = gameName,
                    Player = playerName,
                    Score = score,
                    Date = now,
                    Position = beatenRecord?.Position ?? _fame.Count + 1
                };

                for (var i = _fame.Count - 1; i >= 0; i--)
                {
                    var record = _fame[i];
                    if (record.Position > newRecord.Position || (record.Position == newRecord.Position && record.Score < newRecord.Score))
                    {
                        if (++record.Position > RECORD_COUNT)
                        {
                            _fame.RemoveAt(i);
                        }
                    }
                }
                _fame.Add(newRecord);

                return true;
            }

            return false;
        }

        private bool CheckForShameRecord(string gameName, string playerName, int score, DateTime now)
        {
            var beatenRecord = _shame.FirstOrDefault(r => score <= r.Score);
            if (beatenRecord is not null || _shame.Count < RECORD_COUNT)
            {
                var newRecord = new Record
                {
                    Type = RecordType.Shame,
                    Game = gameName,
                    Player = playerName,
                    Score = score,
                    Date = now,
                    Position = beatenRecord?.Position ?? _shame.Count + 1
                };

                for (var i = _shame.Count - 1; i >= 0; i--)
                {
                    var record = _shame[i];
                    if (record.Position > newRecord.Position || (record.Position == newRecord.Position && record.Score > newRecord.Score))
                    {
                        if (++record.Position > RECORD_COUNT)
                        {
                            _shame.RemoveAt(i);
                        }
                    }
                }
                _shame.Add(newRecord);

                return true;
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