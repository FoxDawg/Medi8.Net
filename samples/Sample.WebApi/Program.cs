using Mediator.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sample.WebApi.Products;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddTransient<ProductsStore>();
builder.Services.AddMediator(cfg =>
{
    cfg.AddHandler<FindProductByIdQuery, FindProductByIdQuery.FindProductByIdQueryHandler>();
    cfg.AddHandler<AddProductCommand, AddProductCommand.AddProductCommandHandler>();
    cfg.AddValidator<AddProductCommand, AddProductCommand.AddProductValidator>();
    cfg.AddValidator<FindProductByIdQuery, FindProductByIdQuery.FindProductByIdQueryValidator>();
});

var app = builder.Build();
app.MapControllers();
app.Run();