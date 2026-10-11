namespace Booking.Domain;

public class User
{
    public int Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public List<BookingDetails> Bookings { get; set; }
}