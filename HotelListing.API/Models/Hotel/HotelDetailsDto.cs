using HotelListing.API.Models.Country;

namespace HotelListing.API.Models.Hotel
{
    public class HotelDetailsDto:BaseHotelDto
    {
        public int Id { get; set; }
        public GetCountryDto Country { get; set; }
    }
}