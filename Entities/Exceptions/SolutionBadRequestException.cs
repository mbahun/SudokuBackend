using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions {
    public sealed class SolutionBadRequestException : BadRequestException{
        public SolutionBadRequestException() : 
            base($"Base64 doesn't decode to byte array of correct size") {
        }
    }
}
