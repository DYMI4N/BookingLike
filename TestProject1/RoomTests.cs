using Xunit;
using BookingLike.Models;
using System.Collections.Generic;

public class RoomTests
{
    [Fact]
    public void Room_ShouldInitialize_WithEmptyAmenitiesAndImages()
    {
        // Arrange
        var room = new Room();

        // Act
        room.Amenities = new List<Amenity>();
        room.Images = new List<RoomImage>();

        // Assert
        Assert.Empty(room.Amenities);
        Assert.Empty(room.Images);
    }

    [Fact]
    public void Room_ShouldHaveCorrectProperties()
    {
        // Arrange
        var room = new Room
        {
            RoomType = "Double",
            Capacity = 2,
            PricePerNight = 150,
            Description = "Pokój dwuosobowy z balkonem"
        };

        // Assert
        Assert.Equal("Double", room.RoomType);
        Assert.Equal(2, room.Capacity);
        Assert.Equal(150, room.PricePerNight);
        Assert.Contains("balkon", room.Description);
    }
}
