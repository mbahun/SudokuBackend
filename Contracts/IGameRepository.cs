using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts {
    public interface IGameRepository {
        Task<Game> CreateNewGameAsync(Game game, bool trackChanges);
        Task<Game> GetGameByIdAsync(Guid gameId, bool trackChanges);
        Task<Game> GetNextAvailableGameForUserAsync(Guid userId, bool trackChanges);
    }
}
