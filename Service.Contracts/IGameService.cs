using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DataTransferObjects;

namespace Service.Contracts {
    public interface IGameService {
        Task<GameDto> CreateNewGameAsync(bool trackChanges);
        Task<GameDto> GetByIdAsnyc(Guid gameId, bool trackChanges);
    }
}
