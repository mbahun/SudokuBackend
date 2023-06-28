using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects {
    public record GameDto {
        public Guid id { get; init; }
        public string? Problem { get; init;}
        public string? Solution { get; init; }
        public DateTime? CreatedAt { get; init; }
    }
}
