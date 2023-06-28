using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects {
    public record UserGameForUpdateDto {
        [Required(ErrorMessage = "Solution is required")]
        public string Solution { get; init; }
    }
}
