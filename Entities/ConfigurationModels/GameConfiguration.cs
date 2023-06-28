using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ConfigurationModels {
    public class GameConfiguration {
        public string Section { get; set; } = "GameConfiguration";
        public uint GameSize { get; set; } = 81;
        public string ExternalAppPath { get; set; } = "";
        public bool UseTestData { get; set; } = true;
    }
}
