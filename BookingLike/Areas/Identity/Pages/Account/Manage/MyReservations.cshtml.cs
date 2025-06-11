using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using BookingLike.Data;
using BookingLike.Models;
using Microsoft.EntityFrameworkCore;

public class MyReservationsModel : PageModel
{
    private readonly BookingLikeDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public MyReservationsModel(BookingLikeDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<Reservation> Reservations { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        Reservations = await _context.Reservations
            .Include(r => r.Room)
            .ThenInclude(r => r.Hotel)
            .Where(r => r.UserId == user.Id)
            .ToListAsync();
    }
}
