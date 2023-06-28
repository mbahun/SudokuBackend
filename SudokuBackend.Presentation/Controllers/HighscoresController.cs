using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using SudokuBackend.Presentation.ActionFilters;
using System.Text.Json;
using Microsoft.AspNetCore.JsonPatch;
using Marvin.Cache.Headers;
using Marvin.Cache.Headers.Interfaces;

namespace SudokuBackend.Presentation.Controllers {
    [ApiVersion("1.0")]
    [Route("api/highscores")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class HighscoresController : ControllerBase {
        private readonly IServiceManager _service;

        public HighscoresController(IServiceManager service) {
            _service = service;
        }


        [HttpGet(Name = "GetUserHighscores")]
        [HttpCacheValidation(MustRevalidate = true)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 30)]
        public async Task<IActionResult> GetUserHighscores([FromQuery] HighscoreParameters highscoresParams) {
            var pagedResult = await _service.HighscoreService.GetUserHighscoresAsync(highscoresParams, false);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
            return Ok(pagedResult.highscores);
        }

    }
}
