using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingLike.Data;
using BookingLike.Models;

namespace BookingLike.Controllers
{
    public class RoomsController : Controller
    {
        private readonly BookingLikeDbContext _context;

        public RoomsController(BookingLikeDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            var bookingLikeDbContext = _context.Rooms.Include(r => r.Hotel);
            return View(await bookingLikeDbContext.ToListAsync());
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // GET: Rooms/Create
        public IActionResult Create(int hotelId)
        {
            ViewBag.Amenities = _context.Amenities
                .Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString() })
                .ToList();

            var room = new Room
            {
                HotelId = hotelId
            };

            return View(room);
        }



        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room, List<IFormFile> ImageFiles, List<int> SelectedAmenityIds)
        {
            if (ImageFiles != null && ImageFiles.Count > 0)
            {
                var uploadsDir = Path.Combine("wwwroot/uploads");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                // zapisz zdjęcia i przypisz pierwszy jako główny
                var imagePaths = new List<string>();

                foreach (var file in ImageFiles)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    imagePaths.Add("/uploads/" + fileName);
                }

                // główne zdjęcie
                room.ImagePath = imagePaths.First();

                // galeria
                room.Images = imagePaths.Select(p => new RoomImage { ImagePath = p }).ToList();
            }

            // udogodnienia
            room.Amenities = _context.Amenities
                .Where(a => SelectedAmenityIds.Contains(a.Id))
                .ToList();

            _context.Add(room);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Amenities)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }
            ViewBag.HotelId = new SelectList(_context.Hotels, "Id", "Id", room.HotelId);
            ViewBag.Amenities = _context.Amenities
                .Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.Id.ToString(),
                    Selected = room.Amenities.Any(ra => ra.Id == a.Id)
                }).ToList();

            return View(room);

        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room room, List<int> SelectedAmenityIds)
        {
            if (id != room.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var roomToUpdate = await _context.Rooms
                        .Include(r => r.Amenities)
                        .FirstOrDefaultAsync(r => r.Id == id);

                    if (roomToUpdate == null)
                        return NotFound();

                    // Zaktualizuj dane podstawowe
                    roomToUpdate.RoomType = room.RoomType;
                    roomToUpdate.Capacity = room.Capacity;
                    roomToUpdate.PricePerNight = room.PricePerNight;
                    roomToUpdate.Description = room.Description;
                    roomToUpdate.HotelId = room.HotelId;
                    roomToUpdate.ImagePath = room.ImagePath;

                    // Zaktualizuj udogodnienia
                    roomToUpdate.Amenities.Clear();
                    roomToUpdate.Amenities = _context.Amenities
                        .Where(a => SelectedAmenityIds.Contains(a.Id)).ToList();

                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.Id))
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

            ViewBag.HotelId = new SelectList(_context.Hotels, "Id", "Id", room.HotelId);
            ViewBag.Amenities = _context.Amenities
                .Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.Id.ToString(),
                    Selected = SelectedAmenityIds.Contains(a.Id)
                }).ToList();


            return View(room);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}
