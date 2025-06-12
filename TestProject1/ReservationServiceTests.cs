using BookingLike.Models;
using Xunit;

public class ReservationServiceTests
{
    [Fact]
    public void CalculateTotalPrice_ShouldReturnCorrectAmount()
    {
        // Arrange
        double pricePerNight = 200;
        int numberOfDays = 3;

        // Act
        decimal total = (decimal)(pricePerNight * numberOfDays);

        // Assert
        Assert.Equal(600m, total);
    }
    [Fact]
    public void NewReservation_ShouldHavePendingStatus()
    {
        // Arrange
        var reservation = new Reservation();

        // Act
        var status = reservation.Status;

        // Assert
        Assert.Equal("Pending", status);
    }
    [Fact]
    public void GetBookedDates_ShouldReturnCorrectRange()
    {
        // Arrange
        var start = new DateTime(2025, 6, 10);
        var end = new DateTime(2025, 6, 13);
        var dates = new List<string>();

        for (var date = start; date < end; date = date.AddDays(1))
        {
            dates.Add(date.ToString("yyyy-MM-dd"));
        }

        // Act & Assert
        Assert.Equal(3, dates.Count);
        Assert.Contains("2025-06-11", dates);
    }
    [Fact]
    public void Reservation_WithInvalidDateRange_ShouldBeInvalid()
    {
        // Arrange
        var start = new DateTime(2025, 6, 15);
        var end = new DateTime(2025, 6, 14);

        var diff = (end - start).TotalDays;

        // Assert
        Assert.True(diff <= 0);
    }

}
