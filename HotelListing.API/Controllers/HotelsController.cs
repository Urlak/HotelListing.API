using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Models.Hotel;
using HotelListing.API.Models.Country;
using HotelListing.API.Models;
using HotelListing.API.Repository;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHotelRepository _hotelRepository;

        public HotelsController(IMapper mapper, IHotelRepository hotelRepository)
        {
            _mapper = mapper;
            _hotelRepository = hotelRepository;
        }

        // GET: api/Hotels/?StartIndex=2&PageSize=1
        [HttpGet]
        public async Task<ActionResult<PagedResult<HotelDto>>> GetHotels([FromQuery] QueryParameters queryParameters)
        {
            return Ok(await _hotelRepository.GetAllAsync<HotelDto>(queryParameters));
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDetailsDto>> GetHotel(int id)
        {
            var hotel = await _hotelRepository.GetDetailsAsync(id);

            if (hotel == null)
            {
                return NotFound();
            }

            return _mapper.Map<HotelDetailsDto>(hotel);
        }

        // PUT: api/Hotels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, HotelDto updateHotelDto)
        {
            if (id != updateHotelDto.Id)
            {
                return BadRequest();
            }

            var hotel = await _hotelRepository.GetAsync(id);
            if (hotel== null)
            {
                return NotFound();
            }
            _mapper.Map(updateHotelDto, hotel);

            try
            {
                await _hotelRepository.UpdateAsync(hotel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _hotelRepository.Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Hotels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDto createHotelDto)
        {
            var hotel = _mapper.Map<Hotel>(createHotelDto);
            await _hotelRepository.AddAsync(hotel);
            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _hotelRepository.GetAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            await _hotelRepository.DeleteAsync(id);
            return NoContent();
        }      
    }
}
