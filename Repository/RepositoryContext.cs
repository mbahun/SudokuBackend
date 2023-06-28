using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repository.Configuration;
using Shared.DataTransferObjects;

namespace Repository {
    public class RepositoryContext : IdentityDbContext<User> {
        public RepositoryContext(DbContextOptions options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //DB seeds
            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            //Many to many with explicit class/table UserGame
            modelBuilder.Entity<User>()
                .HasMany(e => e.Games)
                .WithMany(e => e.Users)
                .UsingEntity<UserGame>();

            modelBuilder.Entity<Highscore>().HasNoKey();
            modelBuilder.Entity<Highscore>().Metadata.SetIsTableExcludedFromMigrations(true);
            modelBuilder.Entity<Highscore>().ToTable(nameof(Highscores), t => t.ExcludeFromMigrations());
  
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<UserGame> UserGame { get; set; }
        public DbSet<Highscore> Highscores { get; set; }
    }
}
