using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Entities.Models {
    public class Game {
        [Column("GameId")]
        public Guid Id { get; set; }
        public string Problem { get; set; }
        public string Solution { get; set; }    
        public uint Ordinal { get; set; }
        public DateTime CreatedAt { get; set; }        
        public ICollection<User>? Users { get; set; } //List or collection? https://stackoverflow.com/questions/7655845/icollectiont-vs-listt-in-entity-framework
    }
}
