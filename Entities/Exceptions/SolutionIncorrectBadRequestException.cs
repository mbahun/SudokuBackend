using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions {
    public sealed class SolutionIncorrectBadRequestException : BadRequestException{
        public SolutionIncorrectBadRequestException() : 
            base($"Solution is not ok") {
        }
    }
}
