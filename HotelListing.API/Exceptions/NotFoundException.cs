namespace HotelListing.API.Exceptions
{
    public class NotFoundException(string name, object key) : ApplicationException($"{name} ({key}) was not found")
    {
    }
}
