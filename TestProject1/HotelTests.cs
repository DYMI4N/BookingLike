using Xunit;
using BookingLike.Models;
using System.Collections.Generic;

public class HotelTests
{
    [Fact]
    public void Hotel_ShouldInitialize_WithEmptyRoomsAndAmenities()
    {
        // Arrange
        var hotel = new Hotel();

        // Act
        hotel.Rooms = new List<Room>();
        hotel.Amenities = new List<Amenity>();

        // Assert
        Assert.Empty(hotel.Rooms);
        Assert.Empty(hotel.Amenities);
    }

    [Fact]
    public void Hotel_ShouldHaveCorrectBasicData()
    {
        // Arrange
        var hotel = new Hotel
        {
            Name = "Hotel Testowy",
            City = "Warszawa",
            Country = "Polska",
            PricePerNight = 300
        };

        // Assert
        Assert.Equal("Hotel Testowy", hotel.Name);
        Assert.Equal("Warszawa", hotel.City);
        Assert.Equal("Polska", hotel.Country);
        Assert.Equal(300, hotel.PricePerNight);
    }
}
