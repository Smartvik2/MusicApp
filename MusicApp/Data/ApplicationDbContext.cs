using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MusicApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MusicApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<ArtistProfile> Artists => Set<ArtistProfile>();
        //[HasNoKey]
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<Appointment>()
                .HasOne(a => a.Artist)
                .WithMany()
                .HasForeignKey(a => a.ArtistId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict); 

            
            builder.Entity<ArtistProfile>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            
            builder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            
            builder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
