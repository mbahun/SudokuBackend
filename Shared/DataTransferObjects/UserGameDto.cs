using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects {
    public record UserGameDto {
        public string UserId { get; init; }
        public Guid GameId { get; init; }
        public string? Solution { get; init; }
        public uint? Score { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? FinishedPlayingAt { get; init; }
    }
}
