namespace BookingLike.Models
{
    public class RoomImage
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
