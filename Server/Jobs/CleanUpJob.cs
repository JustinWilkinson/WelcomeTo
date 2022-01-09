using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using WelcomeTo.Server.Repository;
using WelcomeTo.Shared.Extensions;

namespace WelcomeTo.Server.Jobs
{
    public class CleanUpJob : IJob
    {
        private static readonly IGameRepository _gameRepository;
        private static readonly ILogger<CleanUpJob> _logger;

        static CleanUpJob()
        {
            var loggerFactory = new LoggerFactory();
            _gameRepository = new GameRepository(loggerFactory.CreateLogger<GameRepository>());
            _logger = loggerFactory.CreateLogger<CleanUpJob>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var gameIdsToDelete = await _gameRepository.ListGames(true)
                    .WhereAsync(x => x.CompletedAtUtc.HasValue || x.CreatedAtUtc < DateTime.UtcNow.AddDays(-1) && !x.StartedAtUtc.HasValue || x.StartedAtUtc < DateTime.UtcNow.AddDays(-5))
                    .SelectAsync(x => x.Id)
                    .ToListAsync();

                await _gameRepository.DeleteGames(gameIdsToDelete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred running a clean up job.");
            }
        }
    }
}