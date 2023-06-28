using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestFeatures {
    public class UserGamesParameters : RequestParameters{
        public uint MinScore { get; set; } = 0;
        public uint MaxScore { get; set; } = 60; 
        public DateTime MinFinishedAt { get; set; } = DateTime.MinValue;
        public DateTime MaxFinishedAt { get; set; } = DateTime.UtcNow;
        public bool UnfinishedGames { get; set; } = false;

        public bool ValidScoreRange => MaxScore > MinScore;

        public UserGamesParameters() => OrderBy = "Score";
    }
}
