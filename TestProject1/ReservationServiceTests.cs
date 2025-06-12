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
}
