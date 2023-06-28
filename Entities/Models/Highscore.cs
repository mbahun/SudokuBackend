using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Entities.Models {
    [NotMapped]
    public class Highscore {
        public string Nickname { get; set; }
        public long Score { get; set; }
        public DateTime LastPlayedDate { get; set; }
    }
}
