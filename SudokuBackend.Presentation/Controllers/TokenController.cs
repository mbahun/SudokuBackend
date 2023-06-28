using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using SudokuBackend.Presentation.ActionFilters;

namespace SudokuBackend.Presentation.Controllers {
    [Route("api/token")]
    [ApiController]
    [HttpCacheIgnore]
    public class TokenController : ControllerBase {
        private readonly IServiceManager _service;

        public TokenController(IServiceManager service) {
            _service = service;
        }

        [HttpPost("refresh")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto) {
            var tokenDtoToReturn = await _service.AuthenticationService.RefreshToken(tokenDto);
            return Ok(tokenDtoToReturn);
        }


    }
}
