using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingLike.Data;
using BookingLike.Models;
using Microsoft.AspNetCore.Identity;


namespace BookingLike.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly BookingLikeDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public ReservationsController(BookingLikeDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var bookingLikeDbContext = _context.Reservations.Include(r => r.Room).Include(r => r.User);
            return View(await bookingLikeDbContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public async Task<IActionResult> Create(int roomId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var reservation = new Reservation
            {
                RoomId = roomId,
                UserId = user.Id,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1)
            };

            return View(reservation);
        }


        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,RoomId,UserId,Status,TotalPrice,CreatedAt")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                reservation.UserId = user.Id; // <- przypisujemy ręcznie tutaj

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(reservation);
        }


        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Id", reservation.RoomId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", reservation.UserId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,RoomId,UserId,Status,TotalPrice,CreatedAt")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Id", reservation.RoomId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", reservation.UserId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
#if DEBUG
        [HttpGet]
        public async Task<IActionResult> TestCreate()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Content("User not logged in");

            var testReservation = new Reservation
            {
                RoomId = 1, // zakładamy że pokój o ID = 1 istnieje
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                CreatedAt = DateTime.Now,
                Status = "Pending",
                TotalPrice = 500,
                UserId = user.Id
            };

            _context.Reservations.Add(testReservation);
            await _context.SaveChangesAsync();

            return Content("Rezerwacja testowa została dodana.");
        }
#endif

    }

}

