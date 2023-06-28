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
    internal sealed class UserGameService : IUserGameService {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        
        public UserGameService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<GameDto> CreateUserGameAsync(string userId, bool trackChanges) {
            var gameDto = await GetNextAvailableGameForUserAsync(userId, trackChanges);

            UserGame userGame = new UserGame {
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                GameId = gameDto.id 
            };
            
            _repository.UserGame.CreateUserGame(userGame);
            await _repository.SaveAsync();
            return gameDto;
        }

 
        public async Task<(IEnumerable<UserGameDto> userGames, MetaData metaData)> GetAllUserGamesAsync(
            string userId, UserGamesParameters userGamesParams, bool trackChanges) {

            if (!userGamesParams.ValidScoreRange) {
                throw new ArgumentException(); //ToDo: Add custom exception
            }

            var userGamesWithMetaData = await _repository.UserGame.GetAllUserGamesAsync(userId, userGamesParams, trackChanges);
            var UserGamesDto = _mapper.Map<IEnumerable<UserGameDto>>(userGamesWithMetaData);
            return (userGames: UserGamesDto, metaData: userGamesWithMetaData.MetaData);
        }


        public async Task<UserGameDto> GetUserGameByIdAsync(string userId, Guid gameId, bool trackChanges) {
            var game = await _repository.UserGame.GetUserGameByIdAsync(userId, gameId, trackChanges);
            if (game == null) {
                throw new GameNotFoundException(gameId);
            }

            var userGameDto = _mapper.Map<UserGameDto>(game);
            return userGameDto;
        }


        public async Task<GameDto> GetNextAvailableGameForUserAsync(string userId, bool trackChanges) {
            Guid userGuid = new Guid(userId);

            if (userGuid.Equals(Guid.Empty)) {
                throw new UserIdBadRequest();
            }

            var game = await _repository.Game.GetNextAvailableGameForUserAsync(userGuid, trackChanges);
            if (game == null) {
                throw new NoAvailableGamesForUserException();
            }

            var gameDto = _mapper.Map<GameDto>(game);
            return gameDto;
        }

        
        public async Task<UserGame> GetUserGameForPatch(string userId, Guid gameId, bool trackChanges) {
            var userGame = await _repository.UserGame.GetUserGameByIdAsync(userId, gameId, trackChanges);
            if (userGame == null) {
                throw new GameNotFoundException(gameId);
            }

            return userGame;
        }


        public async Task SaveChangesForPatchAsync(UserGameForUpdateDto userGameToPatch, UserGame userGame) {
            var isSolutionOk = await IsUserGameSolutionOkAsync(userGame.GameId, userGameToPatch.Solution, false);
            if (!isSolutionOk) {
                throw new SolutionIncorrectBadRequestException();
            }

            userGame.Solution = userGameToPatch.Solution;
            userGame.FinishedPlayingAt = DateTime.UtcNow;

            var minutesPlayed = (userGame.FinishedPlayingAt - userGame.CreatedAt).Value.TotalMinutes;

            //Just basic scoring... 
            if (minutesPlayed > 90 || minutesPlayed < 1) {
                minutesPlayed = 89;
            }

            userGame.Score = (uint)(90 - minutesPlayed);   
            await _repository.SaveAsync();
        }


        public async Task<bool> IsUserGameSolutionOkAsync(Guid gameId, string solution, bool trackChanges) {
            var gameDto = await _repository.Game.GetGameByIdAsync(gameId, trackChanges);
            if (gameDto == null) {
                throw new GameNotFoundException(gameId);
            }

            //Check for default solution
            if (gameDto.Solution.Equals(solution)) {
                return true;
            }

            var problemArray = Convert.FromBase64String(gameDto.Problem);
            var solutionArray = Convert.FromBase64String(solution);

            if (problemArray == null || solutionArray == null) {
                throw new SolutionBadRequestException();
            }

            if(problemArray.Length != solutionArray.Length) {
                throw new SolutionBadRequestException();
            }

            //Check if solutuon is a solution to the propblem
            if(!IsSolutionForTheProblem(problemArray, solutionArray)){
                return false;
            }

            //Check if soution is ok
            if (!IsSolutionCorrect(solutionArray)){
                return false;
            }

            return true;
        }


        public bool IsSolutionForTheProblem(byte[] problem, byte[] solution) {
            for (int i=0; i<problem.Length; i++) {
                if ((problem[i] != 0) && (problem[i] != solution[i])) {
                    return false;
                }
            }
            return true;
        }


        public bool IsSolutionCorrect(byte[] solution) {
            int maxNumInCell = (int)Math.Sqrt(solution.Length);
            int[] checks = new int[maxNumInCell + 1];

            for (int i = 0; i < maxNumInCell; i++) {

                //horizontal check
                Array.Clear(checks);
                for (int j = i; j < (i + maxNumInCell); j++) {
                    if (solution[j] == 0 || ++checks[solution[j]] > 1) {
                        return false;
                    }
                }
                //vertical check
                Array.Clear(checks);
                for (int j = i; j <= (maxNumInCell * (maxNumInCell - 1)) + i; j += maxNumInCell) {
                    if (solution[j] == 0 || ++checks[solution[j]] > 1) {
                        return false;
                    }
                }
            }

            return true;
        }


    }
}
