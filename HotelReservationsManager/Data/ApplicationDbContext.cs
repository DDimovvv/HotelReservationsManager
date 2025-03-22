using Microsoft.EntityFrameworkCore;
using Hotel_Reservations_Manager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Hotel_Reservations_Manager.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Reservation>()
                .HasOne(r => r.Client)
                .WithMany(c => c.Reservations)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Reservation>()
                .HasOne(re => re.Room)
                .WithMany(ro => ro.Reservations)
                .HasForeignKey(re => re.RoomNumber)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
