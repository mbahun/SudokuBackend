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
using Shared.DataTransferObjects;
using static System.Formats.Asn1.AsnWriter;
using System.Collections.Immutable;

namespace Repository {
    public class HighscoreRepository : RepositoryBase<UserGame>, IHighscoreRepository {
        public HighscoreRepository(RepositoryContext repositoryContext) : base(repositoryContext) {
        }

    
        public async Task<PagedList<Highscore>> GetUserHighscoresAsync(HighscoreParameters highscoresParams, bool trackChanges) {
            var highscores = await FindAll(trackChanges)
                .Include("User")
                .FilterHigscores(highscoresParams.MinFinishedAt, highscoresParams.MaxFinishedAt)
                .GroupBy(g => g.User.Nickname)
                .Select(s => new Highscore {
                    Nickname = s.Key,
                    Score = s.Sum(sum => sum.Score) ?? 0,
                    LastPlayedDate = s.Max(max => max.FinishedPlayingAt) ?? DateTime.MinValue
                })
                .Sort(highscoresParams.OrderBy)
                .ToListAsync();

            return PagedList<Highscore>
                .ToPagedList(highscores, highscoresParams.PageNumber, highscoresParams.PageSize);
        }


    }
}
