using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions {
    public sealed class UserIdBadRequest : BadRequestException{
        public UserIdBadRequest() : base("User ID is not a valid GUID.") {
        }
    }
}
