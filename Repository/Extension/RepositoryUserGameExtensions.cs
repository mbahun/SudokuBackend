using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using System.Linq.Dynamic.Core;
using Repository.Extensions.Utility;

namespace Repository.Extensions {
    public static class RepositoryUserGameExtensions {

        public static IQueryable<UserGame> FilterGames(
            this IQueryable<UserGame> userGame, uint minScore, uint maxScore, DateTime minFinishedAt, 
                DateTime maxFinishedAt, bool unfinishedGames) {
            
            if (unfinishedGames) {
                return userGame;
            }

            return userGame.Where(e => (e.Score >= minScore && e.Score <= maxScore) 
                && (e.FinishedPlayingAt >= minFinishedAt && e.FinishedPlayingAt <= maxFinishedAt));
        }


        public static IQueryable<UserGame> FilterHigscores(
            this IQueryable<UserGame> userGame, DateTime minFinishedAt, DateTime maxFinishedAt) {

                return userGame.Where(e => e.Score != null &&
                    (e.FinishedPlayingAt >= minFinishedAt && e.FinishedPlayingAt <= maxFinishedAt));
        }


        public static IQueryable<UserGame> Sort(
            this IQueryable<UserGame> userGame, string orderByQueryString) {

            if (string.IsNullOrWhiteSpace(orderByQueryString)) {
                return userGame.OrderBy(e => e.Score);
            }

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<UserGame>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery)) {
                return userGame.OrderBy(e => e.Score);
            }

            return userGame.OrderBy(orderQuery);
        }


    }
}
