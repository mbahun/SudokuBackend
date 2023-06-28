using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service.Contracts {
    public interface IUserGameService {
        Task<GameDto> CreateUserGameAsync(string userId, bool trackChanges);
        Task<(IEnumerable<UserGameDto> userGames, MetaData metaData)> GetAllUserGamesAsync(
            string userId, UserGamesParameters userGamesParams, bool trackChanges);
        Task<UserGameDto> GetUserGameByIdAsync(string userId, Guid gameId, bool trackChanges);
        Task<UserGame> GetUserGameForPatch(string userId, Guid gameId, bool trackChanges);
        Task SaveChangesForPatchAsync(UserGameForUpdateDto userGameToPatch, UserGame userGame);
        Task<bool> IsUserGameSolutionOkAsync(Guid gameId, string solution, bool trackChanges);
    }
}
