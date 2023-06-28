using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using SudokuBackend.Presentation.ActionFilters;

namespace SudokuBackend.Presentation.Controllers {
    [ApiVersion("1.0")]
    [Route("api/games")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [HttpCacheIgnore]
    public class GamesController : ControllerBase {
        private readonly IServiceManager _service;

        public GamesController(IServiceManager service) {
            _service = service;
        }


        [HttpPost(Name = "CreateNewGame")]
        [Authorize(Roles = "Player")]
        public async Task<IActionResult> CreateNewGame() {
            var games = await _service.GameService.CreateNewGameAsync(trackChanges: false);
            return Ok(games);
        }


        [HttpGet("{gameId:guid}", Name = "GetGameById")]
        [Authorize(Roles = "Player")]
        public async Task<IActionResult> GetGameById(Guid gameId) {
            var games = await _service.GameService.GetByIdAsnyc(gameId, trackChanges: false);
            return Ok(games);
        }


    }
}
