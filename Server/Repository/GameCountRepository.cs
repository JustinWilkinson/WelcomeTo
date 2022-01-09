using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WelcomeTo.Shared.Extensions;

namespace WelcomeTo.Server.Repository
{
    /// <summary>
    /// Interface for managing the GameCount table.
    /// </summary>
    public interface IGameCountRepository
    {
        Task<int> GetGameCount();

        Task IncrementGameCount();
    }

    /// <summary>
    /// Manages the GameCount table.
    /// </summary>
    public class GameCountRepository : Repository, IGameCountRepository
    {
        private readonly ILogger<GameCountRepository> _logger;

        public GameCountRepository(ILogger<GameCountRepository> logger)
        {
            _logger = logger;
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                await Execute("CREATE TABLE IF NOT EXISTS GameCount (GameCount integer)");

                if (await ExecuteScalar("SELECT COUNT(*) AS Count FROM GameCount", Convert.ToInt32) == 0)
                {
                    await Execute("INSERT INTO GameCount VALUES (0)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred initialising the Game Count Repository");
                throw;
            }
        }

        public async Task<int> GetGameCount()
        {
            try
            {
                return await Execute("SELECT * FROM GameCount", GetColumnValue("GameCount", Convert.ToInt32)).SingleAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred retrieving the current game count.");
                throw;
            }
        }

        public async Task IncrementGameCount()
        {
            try
            {
                await Execute("UPDATE GameCount SET GameCount = GameCount + 1");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred incrementing the game count.");
                throw;
            }
        }
    }
}