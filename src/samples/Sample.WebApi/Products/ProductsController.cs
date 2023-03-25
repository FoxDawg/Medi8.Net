using System;
using System.Threading;
using System.Threading.Tasks;
using CS.Mediator.Contract;
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
        var result = await this.mediator.HandleQueryAsync<FindProductByIdQuery, Product?>(new FindProductByIdQuery(productId), CancellationToken.None);

        switch (result.StatusCode)
        {
            case CS.Mediator.Contract.StatusCode.Ok when result.Result is null:
                return this.NoContent();
            case CS.Mediator.Contract.StatusCode.Ok:
                return new OkObjectResult(result.Result);
            case CS.Mediator.Contract.StatusCode.BadRequest:
                return new BadRequestObjectResult(result.ValidationResults.ToModelStateDictionary());
            default:
                throw new InvalidOperationException("Case not handled.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Post(string productName)
    {
        var result = await this.mediator.HandleCommandAsync<AddProductCommand, Product>(new AddProductCommand(productName), CancellationToken.None);

        switch (result.StatusCode)
        {
            case CS.Mediator.Contract.StatusCode.Ok:
                return new OkObjectResult(result.Result);
            case CS.Mediator.Contract.StatusCode.BadRequest:
                return new BadRequestObjectResult(result.ValidationResults.ToModelStateDictionary());
            default:
                throw new InvalidOperationException("Case not handled.");
        }
    }
}