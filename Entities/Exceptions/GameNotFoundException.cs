using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions {
    public sealed class GameNotFoundException : NotFoundException{
        public GameNotFoundException(Guid gameId) :
            base($"Game with ID {gameId} not found.") {
        }
    }
}
