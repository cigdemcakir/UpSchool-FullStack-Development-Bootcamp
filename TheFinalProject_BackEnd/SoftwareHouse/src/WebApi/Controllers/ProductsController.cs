using Application.Features.Products.Commands.Add;
using Application.Features.Products.Queries.GetAll;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class ProductsController : ApiControllerBase
    {
        [HttpPost("Add")]
        public async Task<IActionResult> AddAsync(ProductAddCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync(bool? isDeleted)
        {
            var query = new ProductGetAllQuery(isDeleted);
            return Ok(await Mediator.Send(query));
        }
        // [HttpGet("GetAll")]
        // public async Task<IActionResult> GetAllAsync(ProductGetAllQuery query)
        // {
        //     return Ok(await Mediator.Send(query));
        // }
    }
}
