using System.Threading.Tasks;
using Mediator;
using Mediator.Contract;
using Sample.WebApi.Products.Model;

namespace Sample.WebApi.Products;

public record AddProductCommand(string Name) : ICommand<Product>
{
    public class AddProductValidator : IValidateRequest<AddProductCommand>
    {
        public Task<ProcessingResults> ValidateAsync(ProcessingContext<AddProductCommand> context)
        {
            if (string.IsNullOrEmpty(context.Request.Name))
            {
                return Task.FromResult(new ProcessingResults(new[] {new ProcessingResult($"{nameof(Name)}", "Must not be empty") }));
            }

            return Task.FromResult(ProcessingResults.Empty);
        }
    }

    public class AddProductCommandHandler : ICommandHandler<AddProductCommand, Product>
    {
        private readonly ProductsStore store;

        public AddProductCommandHandler(ProductsStore store)
        {
            this.store = store;
        }

        public Task<Product> HandleAsync(ProcessingContext<AddProductCommand, Product> context)
        {
            var product = new Product
            {
                Name = context.Request.Name
            };
            return Task.FromResult(this.store.AddProduct(product));
        }
    }
}