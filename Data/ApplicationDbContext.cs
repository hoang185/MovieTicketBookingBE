using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MovieTicketBooking.DTOs;

namespace MovieTicketBooking.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieType> MovieTypes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Gọi base để Identity hoạt động đúng

            modelBuilder.Entity<Movie>()
                .HasOne(m => m.MovieType)
                .WithMany()
                .HasForeignKey(m => m.MovieTypeId);
        }
    }
}
