using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Entities.Models {
    public class UserGame {
        public string UserId { get; set; }
        public Guid GameId { get; set; }
        public string? Solution { get; set; }
        public uint? Score { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedPlayingAt { get; set; }
        public User User { get; set; } = null!;
        public Game Game { get; set; } = null!;
    }
}
