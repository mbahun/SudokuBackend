using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Service.Contracts;

namespace Service {
    public sealed class ServiceManager : IServiceManager {

        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IGameService> _gameService;
        private readonly Lazy<IUserGameService> _userGameService;
        private readonly Lazy<IHighscoreService> _highscoreService;


        public ServiceManager(
            IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper,
            UserManager<User> userManager, IConfiguration configuration) {

            _authenticationService = new Lazy<IAuthenticationService>(() =>
                new AuthenticationService(logger, mapper, userManager, configuration));

            _gameService = new Lazy<IGameService>(() =>
                new GameService(repositoryManager, logger, mapper, configuration));

            _userGameService = new Lazy<IUserGameService>(() =>
                new UserGameService(repositoryManager, logger, mapper));

            _highscoreService = new Lazy<IHighscoreService>(() =>
                new HighscoreService(repositoryManager, logger, mapper));
        }

 
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IGameService GameService => _gameService.Value;
        public IUserGameService UserGameService => _userGameService.Value;
        public IHighscoreService HighscoreService => _highscoreService.Value;
    }
}
