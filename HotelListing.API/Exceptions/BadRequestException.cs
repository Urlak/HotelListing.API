namespace HotelListing.API.Exceptions
{
    public class BadRequestException(string key1, string key2) : ApplicationException($"Parameters don't match {key1}, {key2}")
    {
    }
}
