using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using AutoMapper;
using HotelListing.API.Core.Models.Hotel;
using HotelListing.API.Core.Models;
using HotelListing.API.Core.Contracts;
using HotelListing.API.Core.Models.Country;
using HotelListing.API.Core.Repository;
using Microsoft.AspNetCore.Authorization;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HotelsController(IMapper mapper, IHotelRepository hotelRepository, ILogger<CountriesController> logger) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
        private readonly IHotelRepository _hotelRepository = hotelRepository;
        private readonly ILogger<CountriesController> _logger = logger;


        // GET: api/Countries/GetAll
        [HttpGet("GetAll")]
        public async Task<ActionResult<PagedResult<GetCountryDto>>> GetCountries()
        {
            return Ok(await _hotelRepository.GetAllAsync<HotelDto>());
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
            return await _hotelRepository.GetDetails(id);
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

            try
            {
                await _hotelRepository.UpdateAsync(id, updateHotelDto);
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
            var hotel = await _hotelRepository.AddAsync<CreateHotelDto, Hotel>(createHotelDto);
            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {            
            await _hotelRepository.DeleteAsync(id);
            return NoContent();
        }      
    }
}
