namespace BookingLike.Models
{
    public class Amenity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public ICollection<Hotel> Hotels { get; set; }
    }
}
