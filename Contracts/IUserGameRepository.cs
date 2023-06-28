using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts {
    public interface IUserGameRepository {
        void CreateUserGame(UserGame userGame);
        Task<PagedList<UserGame>> GetAllUserGamesAsync (
            string userId, UserGamesParameters userGamesParamas, bool trackChanges);
        Task <UserGame> GetUserGameByIdAsync (string userId, Guid gameId, bool trackChanges);
    }
}
