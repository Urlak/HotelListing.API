namespace HotelListing.API.Core.Exceptions
{
    public class NotFoundException(string name, object key) : ApplicationException($"{name} ({key}) was not found")
    {
    }
}
