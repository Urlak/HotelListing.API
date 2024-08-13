using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using HotelListing.API.Core.Models.Country;
using HotelListing.API.Core.Models;
using HotelListing.API.Core.Contracts;
using HotelListing.API.Core.Exceptions;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController(IMapper mapper, ICountriesRepository countriesRepository, ILogger<CountriesController> logger) : ControllerBase
    {

        private readonly IMapper _mapper = mapper;
        private readonly ICountriesRepository _countriesRepository = countriesRepository;
        private readonly ILogger<CountriesController> _logger = logger;

        // GET: api/Countries/GetAll
        [HttpGet("GetAll")]
        public async Task<ActionResult<PagedResult<GetCountryDto>>> GetCountries()
        {
            return Ok(await _countriesRepository.GetAllAsync<GetCountryDto>());
        }

        // GET: api/Countries/?StartIndex=2&PageSize=1
        [HttpGet]
        public async Task<ActionResult<PagedResult<GetCountryDto>>> GetPagedCountries([FromQuery] QueryParameters queryParameters)
        {
            return Ok(await _countriesRepository.GetAllAsync<GetCountryDto>(queryParameters));
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            return await _countriesRepository.GetDetails(id);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
        {
            if (id != updateCountryDto.Id)
            {
                throw new BadRequestException(id.ToString(), updateCountryDto.Id.ToString());
            }

            try
            {
                await _countriesRepository.UpdateAsync(id, updateCountryDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _countriesRepository.Exists(id))
                {
                    _logger.LogWarning($"Country not found for id {id}");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountry)
        {
            var country = await _countriesRepository.AddAsync<CreateCountryDto, GetCountryDto>(createCountry);
            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            await _countriesRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
