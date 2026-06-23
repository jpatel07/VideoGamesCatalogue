using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Config
{
    public class VideoGameConfiguration : IEntityTypeConfiguration<VideoGame>
    {
        public void Configure(EntityTypeBuilder<VideoGame> builder)
        {
            builder.Property(x => x.AggregateRating).HasColumnType("decimal(2,1)");

            builder.ToTable(t => t.HasCheckConstraint(
              "CK_VideoGames_AggregateRating_Range",
              "[AggregateRating] IS NULL OR [AggregateRating] BETWEEN 1.0 AND 5.0"));
        }
    }
}
