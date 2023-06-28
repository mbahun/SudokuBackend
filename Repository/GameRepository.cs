using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Contracts;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Data.SqlClient;
using Shared.DataTransferObjects;

namespace Repository {
    public class GameRepository : RepositoryBase<Game>, IGameRepository {

        public GameRepository(RepositoryContext repositoryContext) : base(repositoryContext) {
        }


        public async Task<Game> CreateNewGameAsync(Game game, bool trackChanges) {
            uint ordinal = await GetLastOrdinalAsync(trackChanges);
            game.Ordinal = ++ordinal;

            Create(game);
            return game;
        }


        public async Task<Game> GetGameByIdAsync(Guid gameId, bool trackChanges) {
            return await FindByCondition(c => c.Id.Equals(gameId), trackChanges).SingleOrDefaultAsync();
        }


        public async Task<Game> GetNextAvailableGameForUserAsync(Guid userId, bool trackChanges) {
            FormattableString sqlCommand = $$"""
                 select GameId, Problem, Solution, Ordinal, CreatedAt from Games g
                 where g.Ordinal = (
                    select min(g2.Ordinal) from Games g2
                    left join (
                        select * from UserGame ug where ug.UserId = {{userId}}
                    ) ug on ug.GameId = g2.GameId
                    where ug.UserId is null)
            """;

            return await ExecuteSql(sqlCommand, trackChanges).FirstOrDefaultAsync();
        }


        private async Task<uint> GetLastOrdinalAsync(bool trackChanges) {
            var count = await FindAll(trackChanges).CountAsync();
            if (count > 0) {
                return await FindAll(trackChanges).MaxAsync(c => c.Ordinal);
            }
            else {
                return 0;
            }
        }


    }
}
