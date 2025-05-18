using BookingLike.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingLike.Data
{
    public class BookingLikeDbContext : DbContext
    {
        public BookingLikeDbContext(DbContextOptions<BookingLikeDbContext> options)
            : base(options)
        {
        }
        public DbSet<Hotel> Hotels { get; set; }

    }
}
