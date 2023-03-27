﻿using System.Threading.Tasks;
using Mediator;
using Mediator.Contract;
using Sample.WebApi.Products.Model;

namespace Sample.WebApi.Products;

public record FindProductByIdQuery(int Id) : IQuery<Product?>
{
    public class FindProductByIdQueryValidator : IValidateRequest<FindProductByIdQuery>
    {
        public Task<ProcessingResults> ValidateAsync(ProcessingContext<FindProductByIdQuery> context)
        {
            if (context.Request.Id < 0)
            {
                return Task.FromResult(new ProcessingResults(new[] {new ProcessingResult($"{nameof(Id)}", "Must not be negative")}));
            }

            return Task.FromResult(ProcessingResults.Empty);
        }
    }

    public class FindProductByIdQueryHandler : IQueryHandler<FindProductByIdQuery, Product?>
    {
        private readonly ProductsStore store;

        public FindProductByIdQueryHandler(ProductsStore store)
        {
            this.store = store;
        }

        public Task<Product?> HandleAsync(ProcessingContext<FindProductByIdQuery, Product?> context)
        {
            return Task.FromResult(this.store.FindById(context.Request.Id));
        }
    }
}