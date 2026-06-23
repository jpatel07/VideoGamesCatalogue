using Core.Entities;
using Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data
{
    public class GamesCatalogueContext : DbContext
    {

        public DbSet<VideoGame> VideoGames { get; set; }

        public GamesCatalogueContext(DbContextOptions options) : base(options)
        {
        }

        protected GamesCatalogueContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VideoGameConfiguration).Assembly);
        }
    }
}
