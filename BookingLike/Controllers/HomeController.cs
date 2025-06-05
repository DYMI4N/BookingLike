using BookingLike.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookingLike.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookingLikeDbContext _context;

        public HomeController(BookingLikeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hotels = await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.Amenities)
                .ToListAsync();

            ViewBag.HotelMinPrices = hotels.ToDictionary(
                h => h.Id,
                h => h.Rooms.Any() ? h.Rooms.Min(r => r.PricePerNight) : 0
            );

            return View(hotels);
        }
    }
}
