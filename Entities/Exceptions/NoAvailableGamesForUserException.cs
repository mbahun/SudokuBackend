using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions {
    public sealed class NoAvailableGamesForUserException : NotFoundException{
        public NoAvailableGamesForUserException() :
            base($"There are no available games, create a new one.") {
        }
    }
}
