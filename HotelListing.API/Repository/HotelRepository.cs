using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository
{
    public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
    {
        private readonly HotelListringDbContext _context;

        public HotelRepository(HotelListringDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

        public async Task<Hotel> GetDetailsAsync(int id) => await _context.Hotels.Include(hotel => hotel.Country).FirstOrDefaultAsync(q => q.Id == id);
    }
}
