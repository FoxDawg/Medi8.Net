using System;
using System.Threading.Tasks;
using Mediator.Contract;
using Microsoft.AspNetCore.Mvc;
using Sample.WebApi.Products.Model;

namespace Sample.WebApi.Products;

[ApiController]
[Route("[controller]")]
public class ProductsController : Controller
{
    private readonly IMediator mediator;

    public ProductsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<Product>> Get(int productId)
    {
        var result = await this.mediator.HandleQueryAsync<FindProductByIdQuery, Product?>(new FindProductByIdQuery(productId), HttpContext.RequestAborted);

        switch (result.StatusCode)
        {
            case StatusCodes.Ok when result.Result is null:
                return this.NoContent();
            case StatusCodes.Ok:
                return new OkObjectResult(result.Result);
            case StatusCodes.ValidationFailed:
                return new BadRequestObjectResult(result.Errors.ToModelStateDictionary());
            default:
                throw new InvalidOperationException("Case not handled.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Post(string productName)
    {
        var result = await this.mediator.HandleCommandAsync<AddProductCommand, Product>(new AddProductCommand(productName), HttpContext.RequestAborted);

        switch (result.StatusCode)
        {
            case StatusCodes.Ok:
                return new OkObjectResult(result.Result);
            case StatusCodes.ValidationFailed:
                return new BadRequestObjectResult(result.Errors.ToModelStateDictionary());
            default:
                throw new InvalidOperationException("Case not handled.");
        }
    }
}