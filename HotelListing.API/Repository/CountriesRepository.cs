using HotelListing.API.Contracts;
using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository
{
    public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
    {
        private readonly HotelListringDbContext _context;

        public CountriesRepository(HotelListringDbContext context) : base(context)
        {
            this._context = context;
        }

        public async Task<Country> GetDetailsAsync(int id) => await _context.Countries.Include(q => q.Hotels).FirstOrDefaultAsync(q => q.Id == id);
    }
}
