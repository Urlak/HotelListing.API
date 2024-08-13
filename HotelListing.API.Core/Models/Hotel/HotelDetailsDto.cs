using HotelListing.API.Core.Models.Country;

namespace HotelListing.API.Core.Models.Hotel
{
    public class HotelDetailsDto : BaseHotelDto
    {
        public int Id { get; set; }
        public GetCountryDto Country { get; set; }
    }
}