namespace BookingLike.Models
{
    public class Room
    {

        public int Id { get; set; }
        public string RoomType { get; set; } // e.g., Single, Double, Suite
        public int Capacity { get; set; } // Number of guests the room can accommodate
        public double PricePerNight { get; set; } // Price per night for the room
        public string Description { get; set; } // Description of the room
        public int HotelId { get; set; } // Foreign key to the hotel
        public Hotel Hotel { get; set; } // Navigation property to the hotel
        public string ImagePath { get; set; } // Path to the room image
        public ICollection<Amenity> Amenities { get; set; } // Amenities available in the room
        public ICollection<RoomImage> Images { get; set; }

    }
}
