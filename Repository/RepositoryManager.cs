using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Repository {
    public sealed class RepositoryManager : IRepositoryManager {
        private readonly RepositoryContext _repositoryContext;

        private readonly Lazy<IGameRepository> _gameRepository;
        private readonly Lazy<IUserGameRepository> _userGameRepository;
        private readonly Lazy<IHighscoreRepository> _highscoreRepository;

        public RepositoryManager(RepositoryContext repositoryContext) {
            _repositoryContext = repositoryContext;

            _gameRepository = new Lazy<IGameRepository>(() =>
                new GameRepository(repositoryContext));

            _userGameRepository = new Lazy<IUserGameRepository>(() =>
                new UserGameRepository(repositoryContext));

            _highscoreRepository = new Lazy<IHighscoreRepository>(() =>
                new HighscoreRepository(repositoryContext));    
        }

        public IGameRepository Game => _gameRepository.Value;
        public IUserGameRepository UserGame => _userGameRepository.Value;
        public IHighscoreRepository Highscore => _highscoreRepository.Value;

        public async Task SaveAsync() {
            await _repositoryContext.SaveChangesAsync();
        }
    }
}
