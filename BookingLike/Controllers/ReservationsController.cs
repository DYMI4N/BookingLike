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

            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
            {
                return NotFound();
            }

            var reservation = new Reservation
            {
                RoomId = roomId,
                UserId = user.Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            };

            ViewBag.PricePerNight = room.PricePerNight;

            var reservations = await _context.Reservations
                .Where(r => r.RoomId == roomId && r.Status != "Cancelled")
                .ToListAsync();

            var bookedDates = new List<string>();
            foreach (var res in reservations)
            {
                for (var date = res.StartDate.Date; date < res.EndDate.Date; date = date.AddDays(1))
                {
                    bookedDates.Add(date.ToString("yyyy-MM-dd"));
                }
            }

            ViewBag.BookedDates = bookedDates;

            return View(reservation);
        }





        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            ModelState.Remove("User");
            ModelState.Remove("UserId");
            ModelState.Remove("Room");

            if (ModelState.IsValid)
            {
                reservation.StartDate = reservation.StartDate.Date;
                reservation.EndDate = reservation.EndDate.Date;

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                bool isOverlapping = await _context.Reservations
                    .AnyAsync(r =>
                        r.RoomId == reservation.RoomId &&
                        r.Status != "Cancelled" &&
                        reservation.StartDate < r.EndDate &&
                        reservation.EndDate > r.StartDate
                    );

                if (isOverlapping)
                {
                    ModelState.AddModelError(string.Empty, "Wybrany pokój jest już zarezerwowany w tym terminie.");
                    return View(reservation);
                }

                var room = await _context.Rooms.FindAsync(reservation.RoomId);
                if (room == null)
                {
                    ModelState.AddModelError("", "Nie znaleziono pokoju.");
                    return View(reservation);
                }
                var days = (decimal)(reservation.EndDate - reservation.StartDate).TotalDays;
                if (days <= 0)
                {
                    ModelState.AddModelError("", "Nieprawidłowy zakres dat.");
                    return View(reservation);
                }

                reservation.TotalPrice = Math.Round(Decimal.Multiply((decimal)room.PricePerNight, days),2);
                reservation.UserId = user.Id;
                reservation.CreatedAt = DateTime.Now;
                reservation.Status = "Pending";

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                TempData["ReservationSuccess"] = true;
                return RedirectToAction("Index", "Hotels");

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null || reservation.Status != "Pending")
            {
                return NotFound();
            }

            reservation.Status = "Paid";
            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/Manage/MyReservations", new { area = "Identity" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null || (reservation.Status != "Pending" && reservation.Status != "Paid"))
            {
                return NotFound();
            }

            reservation.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/Manage/MyReservations", new { area = "Identity" });
        }



    }

}

