using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Contracts {
    public interface IHighscoreRepository {
        Task<PagedList<Highscore>> GetUserHighscoresAsync (HighscoreParameters highscoresParams, bool trackChanges);
    }
}
