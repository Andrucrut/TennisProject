using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TennisProject.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        public TennisDbContext tennisDb { get; set; }
        public CityController(TennisDbContext tennisDb)
        {
            this.tennisDb = tennisDb;
        }
        [AllowAnonymous]
        [HttpGet("GetCities")]
        // [CheckInitData]
        public async Task<List<City>> GetCities([FromServices] InitDataValidator initDataValidator)
        {
            //var message = $"User registered successfully id = {user.Id}, username = {user.Username}";
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            //if (validatorResponse.IsValidated == false)
            //    return Ok("not validated");


            return await tennisDb.Cities.ToListAsync();
        }

    }
}
