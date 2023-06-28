using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using System.Linq.Dynamic.Core;
using Repository.Extensions.Utility;

namespace Repository.Extensions {
    public static class RepositoryHighscoreExtensions {

        public static IQueryable<Highscore> Sort(
            this IQueryable<Highscore> highscore, string orderByQueryString) {

            if (string.IsNullOrWhiteSpace(orderByQueryString)) {
                return highscore.OrderBy(e => e.Score);
            }

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Highscore>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery)) {
                return highscore.OrderBy(e => e.Score);
            }

            return highscore.OrderBy(orderQuery);
        }
        

    }
}
