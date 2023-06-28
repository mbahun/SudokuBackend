using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Contracts;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Shared.RequestFeatures;
using System.ComponentModel.Design;
using Repository.Extensions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq.Dynamic.Core;

namespace Repository {
    public class UserGameRepository : RepositoryBase<UserGame>, IUserGameRepository {
        public UserGameRepository(RepositoryContext repositoryContext) : base(repositoryContext) {
        }


        public void CreateUserGame(UserGame userGame) {
            Create(userGame);
        }

    
        public async Task<PagedList<UserGame>> GetAllUserGamesAsync(string userId, UserGamesParameters userGamesParams, bool trackChanges) {
            var userGames = await FindByCondition(e => e.UserId.Equals(userId), trackChanges)
                .FilterGames(userGamesParams.MinScore, userGamesParams.MaxScore, userGamesParams.MinFinishedAt, 
                             userGamesParams.MaxFinishedAt, userGamesParams.UnfinishedGames)
                .Sort(userGamesParams.OrderBy)
                .ToListAsync();
            
            return PagedList<UserGame>
                .ToPagedList(userGames, userGamesParams.PageNumber, userGamesParams.PageSize);
        }


        public async Task<UserGame> GetUserGameByIdAsync(string userId, Guid gameId, bool trackChanges) {
            var userGame = await FindByCondition(
                e => e.UserId.Equals(userId) && e.GameId.Equals(gameId), trackChanges).SingleOrDefaultAsync();

            return userGame;
        }


    }
}
