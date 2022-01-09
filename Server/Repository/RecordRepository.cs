using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        Task<IEnumerable<Record>> ListRecords();

        Task UpdateRecords(Game records);
    }

    /// <summary>
    /// Manages the GameCount table.
    /// </summary>
    public class RecordRepository : Repository, IRecordRepository
    {
        private const int RecordCount = 5;

        private readonly ILogger<RecordRepository> _logger;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private List<Record> _fame = new();
        private List<Record> _shame = new();

        public RecordRepository(ILogger<RecordRepository> logger)
        {
            _logger = logger;
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                await _semaphore.WaitAsync();

                await Execute("CREATE TABLE IF NOT EXISTS Records (RecordsJson text)");

                if (await ExecuteScalar("SELECT COUNT(*) AS Count FROM Records", Convert.ToInt32) == 0)
                {
                    await Execute("INSERT INTO Records VALUES ('')");
                }
                else
                {
                    var records = await Execute("SELECT * FROM Records", DeserializeColumn<IEnumerable<Record>>("RecordsJson")).SingleOrDefaultAsync();
                    records.ForEach(r => (r.Type == RecordType.Fame ? _fame : _shame).Add(r));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred initialising the Record Repository");
                throw;
            }
            finally 
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<Record>> ListRecords()
        {
            await InitializeIfRequiredAsync();

            try
            {
                await _semaphore.WaitAsync();

                OrderRecords();
                return _fame.Concat(_shame);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred retrieving records.");
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task UpdateRecords(Game game)
        {
            await InitializeIfRequiredAsync();

            try
            {
                await _semaphore.WaitAsync();

                var unsavedChanges = false;
                var now = DateTime.UtcNow;
                foreach (var player in game.Players)
                {
                    var score = game.GetPointsTotal(player);
                    if (CheckForFameRecord(game.Name, player.Name, score, now) | CheckForShameRecord(game.Name, player.Name, score, now)) // Use bitwise OR, want to always check both.
                    {
                        OrderRecords();
                        unsavedChanges = true;
                    }
                }

                if (unsavedChanges)
                {
                    await UpdateRecords();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred updating the records.");
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        #region Private
        private async Task UpdateRecords()
        {
            try
            {
                var command = new SQLiteCommand("UPDATE Records SET RecordsJson = @RecordsJson");
                command.AddParameter("@RecordsJson", _fame.Concat(_shame).Serialize());
                await Execute(command);
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
            if (beatenRecord is not null || _fame.Count < RecordCount)
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
                        if (++record.Position > RecordCount)
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
            if (beatenRecord is not null || _shame.Count < RecordCount)
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
                        if (++record.Position > RecordCount)
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