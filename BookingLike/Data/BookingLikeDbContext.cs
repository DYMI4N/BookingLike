using BookingLike.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BookingLike.Data
{
    public class BookingLikeDbContext : IdentityDbContext<IdentityUser>
    {
        public BookingLikeDbContext(DbContextOptions<BookingLikeDbContext> options)
            : base(options)
        {
        }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

    }
}
