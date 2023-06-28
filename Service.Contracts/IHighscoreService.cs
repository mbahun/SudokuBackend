using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service.Contracts {
    public interface IHighscoreService {

        Task<(IEnumerable<HighscoreDto> highscores, MetaData metaData)> GetUserHighscoresAsync(
            HighscoreParameters highscoresParams, bool trackChanges);
      
    }
}
