using Application.Features.Cities.Commands.Add;
using Application.Features.Cities.Queries.GetAll;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers
{

    public class CitiesController : ApiControllerBase
    {
        [HttpPost]
        [ValidationFilter]
        public async Task<IActionResult> AddAsync(CityAddCommand command)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y=>y.Count>0)
                    .ToList();
                return BadRequest(ModelState);
            }

            return Ok(await Mediator.Send(command));
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAllAsync(CityGetAllQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            return Ok(await Mediator.Send(new CityGetAllQuery(id,null)));
        }
    }
}
