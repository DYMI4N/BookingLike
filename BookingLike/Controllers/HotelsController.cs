using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookingLike.Data;
using BookingLike.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;


namespace BookingLike.Controllers
{
    public class HotelsController : Controller
    {
        private readonly BookingLikeDbContext _context;

        public HotelsController(BookingLikeDbContext context)
        {
            _context = context;
        }

        // GET: Hotels
        public async Task<IActionResult> Index(string sortOrder)
        {
            var hotels = await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.Amenities)
                .ToListAsync();

            var hotelMinPrices = hotels.ToDictionary(
                h => h.Id,
                h => h.Rooms.Any() ? h.Rooms.Min(r => r.PricePerNight) : 0.0
            );

            hotels = sortOrder switch
            {
                "price_asc" => hotels.OrderBy(h => hotelMinPrices[h.Id]).ToList(),
                "price_desc" => hotels.OrderByDescending(h => hotelMinPrices[h.Id]).ToList(),
                "name_desc" => hotels.OrderByDescending(h => h.Name).ToList(),
                "name_asc" or _ => hotels.OrderBy(h => h.Name).ToList(),
            };

            ViewBag.HotelMinPrices = hotelMinPrices;
            ViewBag.CurrentSort = sortOrder;

            return View(hotels);
        }

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }



        // GET: Hotels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.Amenities)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hotel == null)
            {
                return NotFound();
            }

            ViewBag.MinPrice = hotel.Rooms.Any() ? hotel.Rooms.Min(r => r.PricePerNight) : 0;

            try
            {
                string apiKey = "6dae3632795044e6bca64044250506";
                string city = RemoveDiacritics(hotel.City + ", Poland");
                string encodedCity = Uri.EscapeDataString(city);


                if (!string.IsNullOrWhiteSpace(city))
                {
                    using var httpClient = new HttpClient();
                    string url = $"https://api.weatherapi.com/v1/forecast.json?key={apiKey}&q={encodedCity}&days=14&aqi=no&alerts=no";

                    var response = await httpClient.GetStringAsync(url);
                    var weatherData = JsonDocument.Parse(response);

                    ViewBag.Forecast = weatherData.RootElement
                        .GetProperty("forecast")
                        .GetProperty("forecastday");
                }
            }
            catch
            {
                ViewBag.Forecast = null;
            }

            return View(hotel);
        }



        // GET: Hotels/Create
        public IActionResult Create()
        {
            ViewBag.Amenities = _context.Amenities
                .Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString() })
                .ToList();

            return View();
        }

        // POST: Hotels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hotel hotel, IFormFile imageFile,List<int>SelectedAmenityIds)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ViewBag.ImageError = "Zdjęcie hotelu jest wymagane.";
                ViewBag.Amenities = _context.Amenities
                    .Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString() })
                    .ToList();
                return View(hotel);
            }

            var fileName = Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine("wwwroot/uploads", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            hotel.Amenities = _context.Amenities
                .Where(a => SelectedAmenityIds.Contains(a.Id))
                .ToList();

            hotel.ImagePath = "/uploads/" + fileName;

            _context.Add(hotel);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }






        // GET: Hotels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .Include(h => h.Amenities)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null)
            {
                return NotFound();
            }

            ViewBag.Amenities = _context.Amenities
                .Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.Id.ToString()
                }).ToList();

            return View(hotel);
        }


        // POST: Hotels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hotel hotel, List<int> SelectedAmenityIds)
        {
            if (id != hotel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var hotelToUpdate = await _context.Hotels
                        .Include(h => h.Amenities)
                        .FirstOrDefaultAsync(h => h.Id == id);

                    if (hotelToUpdate == null)
                        return NotFound();

                    // aktualizacja pól ręcznie
                    hotelToUpdate.Name = hotel.Name;
                    hotelToUpdate.Address = hotel.Address;
                    hotelToUpdate.City = hotel.City;
                    hotelToUpdate.Country = hotel.Country;
                    hotelToUpdate.Description = hotel.Description;
                    hotelToUpdate.ContactNumber = hotel.ContactNumber;
                    hotelToUpdate.Email = hotel.Email;

                    // zaktualizuj Amenities
                    var selectedAmenities = await _context.Amenities
                        .Where(a => SelectedAmenityIds.Contains(a.Id))
                        .ToListAsync();

                    // EF śledzi zmiany – nie przypisuj nowej listy, tylko modyfikuj istniejącą
                    hotelToUpdate.Amenities.Clear();
                    foreach (var amenity in selectedAmenities)
                    {
                        hotelToUpdate.Amenities.Add(amenity);
                    }

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotel.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewBag.Amenities = _context.Amenities
                .Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.Id.ToString()
                }).ToList();

            return View(hotel);
        }




        // GET: Hotels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel != null)
            {
                _context.Hotels.Remove(hotel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HotelExists(int id)
        {
            return _context.Hotels.Any(e => e.Id == id);
        }
    }
}
