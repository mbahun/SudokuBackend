using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestFeatures {
    public class HighscoreParameters : RequestParameters{
        public DateTime MinFinishedAt { get; set; } = DateTime.MinValue;
        public DateTime MaxFinishedAt { get; set; } = DateTime.UtcNow;
        public HighscoreParameters() => OrderBy = "Score desc";
    }
}
