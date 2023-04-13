using System.Threading.Tasks;
using Mediator;
using Mediator.Contract;
using Sample.WebApi.Products.Model;

namespace Sample.WebApi.Products;

public record FindProductByIdQuery(int Id) : IQuery<Product?>
{
    public class FindProductByIdQueryValidator : IValidator<FindProductByIdQuery>
    {
        public Task<Errors> ValidateAsync(IProcessingContext<FindProductByIdQuery> context)
        {
            if (context.Request.Id < 0)
            {
                return Task.FromResult(new Errors(new[] { new Error($"{nameof(Id)}", "Must not be negative") }));
            }

            return Task.FromResult(Errors.Empty);
        }
    }

    public class FindProductByIdQueryHandler : IQueryHandler<FindProductByIdQuery, Product?>
    {
        private readonly ProductsStore store;

        public FindProductByIdQueryHandler(ProductsStore store)
        {
            this.store = store;
        }

        public Task<Product?> HandleAsync(IProcessingContext<FindProductByIdQuery> context)
        {
            return Task.FromResult(this.store.FindById(context.Request.Id));
        }
    }
}