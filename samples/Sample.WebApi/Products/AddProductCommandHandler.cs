using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Mediator.Contract;
using Mediator.Handler;
using Sample.WebApi.Products.Model;

namespace Sample.WebApi.Products;

public record AddProductCommand(string Name) : ICommand<Product>
{
    public class AddProductCommandHandler : CommandHandlerBase<AddProductCommand, Product>
    {
        private readonly ProductsStore store;

        public AddProductCommandHandler(ProductsStore store)
        {
            this.store = store;
        }

        public override Task<ValidationResults> ValidateAsync(AddProductCommand command, CancellationToken token)
        {
            if (string.IsNullOrEmpty(command.Name))
            {
                return Task.FromResult(new ValidationResults(new[] {new ValidationResult($"{nameof(Name)}", "Must not be empty") }));
            }

            return Task.FromResult(ValidationResults.Empty);
        }

        public override Task<Product> HandleAsync(ProcessingContext<AddProductCommand, Product> context)
        {
            var product = new Product
            {
                Name = context.Request.Name
            };
            return Task.FromResult(this.store.AddProduct(product));
        }
    }
}