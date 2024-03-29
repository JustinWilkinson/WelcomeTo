﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WelcomeTo.Server.Repository;
using WelcomeTo.Shared.Extensions;

namespace WelcomeTo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordController : ControllerBase
    {
        private readonly ILogger<RecordController> _logger;
        private readonly IRecordRepository _recordRepository;

        public RecordController(ILogger<RecordController> logger, IRecordRepository gameRepository)
        {
            _logger = logger;
            _recordRepository = gameRepository;
        }

        [HttpGet("List")]
        public async Task<string> List() => (await _recordRepository.ListRecords()).Serialize();
    }
}