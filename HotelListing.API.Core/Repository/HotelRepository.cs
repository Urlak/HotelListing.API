using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.API.Core.Contracts;
using HotelListing.API.Core.Exceptions;
using HotelListing.API.Core.Models.Country;
using HotelListing.API.Core.Models.Hotel;
using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Core.Repository
{
    public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
    {
        private readonly HotelListringDbContext _context;
        private readonly IMapper _mapper;

        public HotelRepository(HotelListringDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<HotelDetailsDto> GetDetails(int id)
        {
            return await _context.Hotels
                .Include(hotel => hotel.Country)
                .ProjectTo<HotelDetailsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(q => q.Id == id)
                  ?? throw new NotFoundException(nameof(GetDetails), id);
        }
    }
}
