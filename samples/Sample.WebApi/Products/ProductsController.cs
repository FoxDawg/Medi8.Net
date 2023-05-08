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
        var result = await this.mediator.HandleQueryAsync<FindProductByIdQuery, Product?>(new FindProductByIdQuery(productId), this.HttpContext.RequestAborted);

        switch (result.Status)
        {
            case Status.OkStatus when result.Result is null:
                return this.NoContent();
            case Status.OkStatus:
                return new OkObjectResult(result.Result);
            case Status.ValidationFailedStatus:
                return new BadRequestObjectResult(result.Errors.ToModelStateDictionary());
            default:
                throw new InvalidOperationException("Case not handled.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Post(string productName)
    {
        var result = await this.mediator.HandleCommandAsync<AddProductCommand, Product>(new AddProductCommand(productName), this.HttpContext.RequestAborted);

        switch (result.Status)
        {
            case Status.OkStatus:
                return new OkObjectResult(result.Result);
            case Status.ValidationFailedStatus:
                return new BadRequestObjectResult(result.Errors.ToModelStateDictionary());
            default:
                throw new InvalidOperationException("Case not handled.");
        }
    }
}