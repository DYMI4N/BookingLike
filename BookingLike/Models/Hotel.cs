﻿namespace BookingLike.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public double PricePerNight { get; set; }
        public string Description { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; } // Path to the hotel image
        public ICollection<Room> Rooms { get; set; }
        public ICollection<Amenity> Amenities { get; set; } 

    }
}
