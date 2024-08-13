using HotelListing.API.Data;

namespace HotelListing.API.Core.Contracts
{
    public interface IHotelRepository : IGenericRepository<Hotel>
    {
        Task<Hotel> GetDetailsAsync(int id);
    }
}
