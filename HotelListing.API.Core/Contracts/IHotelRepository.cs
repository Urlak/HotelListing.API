using HotelListing.API.Core.Models.Hotel;
using HotelListing.API.Data;

namespace HotelListing.API.Core.Contracts
{
    public interface IHotelRepository : IGenericRepository<Hotel>
    {
        Task<HotelDetailsDto> GetDetails(int id);
    }
}
