using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Entities;

namespace MovieTicketBooking.Data.EntityConfig
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasOne(m => m.MovieType)
                .WithMany()
                .HasForeignKey(m => m.MovieTypeId);

            builder.HasOne(m => m.AppRating)
                .WithMany()
                .HasForeignKey(m => m.AppRatingId);
        }
    }
}
