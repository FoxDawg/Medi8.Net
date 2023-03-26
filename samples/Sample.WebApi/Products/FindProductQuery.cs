using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Mediator.Contract;
using Mediator.Handler;
using Sample.WebApi.Products.Model;

namespace Sample.WebApi.Products;

public record FindProductByIdQuery(int Id) : IQuery<Product?>
{
    public class FindProductByIdQueryHandler : QueryHandlerBase<FindProductByIdQuery, Product?>
    {
        private readonly ProductsStore store;

        public FindProductByIdQueryHandler(ProductsStore store)
        {
            this.store = store;
        }

        public override Task<ValidationResults> ValidateAsync(FindProductByIdQuery query, CancellationToken token)
        {
            if (query.Id < 0)
            {
                return Task.FromResult(new ValidationResults(new[] {new ValidationResult($"{nameof(Id)}", "Must not be negative") }));
            }

            return Task.FromResult(ValidationResults.Empty);
        }

        public override Task<Product?> HandleAsync(ProcessingContext<FindProductByIdQuery, Product?> context)
        {
            return Task.FromResult(this.store.FindById(context.Request.Id));
        }
    }
}