using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service {
    internal sealed class HighscoreService : IHighscoreService {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public HighscoreService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }


        public async Task<(IEnumerable<HighscoreDto> highscores, MetaData metaData)> GetUserHighscoresAsync(
            HighscoreParameters highscoresParams, bool trackChanges) {

            var highscores = await _repository.Highscore.GetUserHighscoresAsync(highscoresParams, trackChanges);
            var highscoresDto = _mapper.Map<IEnumerable<HighscoreDto>>(highscores);
            return (highscoresDto, highscores.MetaData);
        }


    }
}
