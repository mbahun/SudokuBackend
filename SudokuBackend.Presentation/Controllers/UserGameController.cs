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
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SudokuBackend.Presentation.Controllers {
    [ApiVersion("1.0")]
    [Route("api/user_game")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class UserGameController : ControllerBase {
        private readonly IServiceManager _service;
        private readonly IValidatorValueInvalidator _validatorValueInvalidator;
        private readonly IStoreKeyAccessor _storeKeyAccessor;

        public UserGameController(
            IServiceManager service, IValidatorValueInvalidator validatorValueInvalidator, IStoreKeyAccessor storeKeyAccessor) {

            _validatorValueInvalidator = validatorValueInvalidator;
            _storeKeyAccessor = storeKeyAccessor;
            _service = service;
        }


        [HttpPost(Name = "CreateUserGame")]
        [Authorize(Roles = "Player")]
        public async Task<IActionResult> CreatePlayerGame() {
            var userId = _service.AuthenticationService.GetCurrUserIdAsync(HttpContext.User);
            var games = await _service.UserGameService.CreateUserGameAsync(userId, false);
            return Ok(games);
        }


        [HttpGet("{gameId:guid}", Name = "GetUserGameById")]
        [Authorize(Roles = "Player")]
        public async Task<IActionResult> GetUserGameById(Guid gameId) {
            var userId = _service.AuthenticationService.GetCurrUserIdAsync(HttpContext.User);
            var games = await _service.UserGameService.GetUserGameByIdAsync(userId, gameId, false);
            return Ok(games);
        }

        [HttpGet(Name = "GetUserGames")]
        [Authorize(Roles = "Player")]
        public async Task<IActionResult> GetUserGames([FromQuery] UserGamesParameters userGamesParams) {
            var userId = _service.AuthenticationService.GetCurrUserIdAsync(HttpContext.User);
            var pagedResult = await _service.UserGameService.GetAllUserGamesAsync(userId, userGamesParams, false);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
            return Ok(pagedResult.userGames);
        }


        [HttpPost("{gameId:guid}/solution", Name = "CheckUserGameSolution")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [Authorize(Roles = "Player")]
        public async Task<IActionResult> IsUserGameSolutionOk(Guid gameId, [FromBody] SolutionForCheckingDto solution) {
            bool ret = await _service.UserGameService.IsUserGameSolutionOkAsync(gameId, solution.Solution, false);

            return Ok(new { isSolutionOk = ret });
        }


        [HttpPatch("{gameId:guid}", Name = "PatchUserGameWithSolution")]
        [Authorize(Roles = "Player")]
        [HttpCacheValidation(ProxyRevalidate = true)]
        public async Task<IActionResult> PatchUserGameWithSolution(Guid gameId, [FromBody] JsonPatchDocument<UserGameForUpdateDto> patchDoc) {
            if (patchDoc is null) {
                return BadRequest("patchDoc object sent from client is null.");
            }

            var userId = _service.AuthenticationService.GetCurrUserIdAsync(HttpContext.User);
            var userGame = await _service.UserGameService.GetUserGameForPatch(userId, gameId, true);
            UserGameForUpdateDto userGameForUpdate = new UserGameForUpdateDto();

            patchDoc.ApplyTo(userGameForUpdate, ModelState);

            await _service.UserGameService.SaveChangesForPatchAsync(userGameForUpdate, userGame);

            /*
            //TryValidateModel(userGameForUpdate);

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            */

            //Invalidate highscore cache
            //As suggested here: https://github.com/KevinDockx/HttpCacheHeaders/tree/master
            var highscoresCache = await _storeKeyAccessor.FindByKeyPart("api/highscores");
            await _validatorValueInvalidator.MarkForInvalidation(highscoresCache);

            return NoContent();
        }

    }
}
