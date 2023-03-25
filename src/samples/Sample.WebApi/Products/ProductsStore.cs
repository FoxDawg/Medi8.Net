using System.Collections.Concurrent;
using Sample.WebApi.Products.Model;

namespace Sample.WebApi.Products;

public class ProductsStore
{
    private static readonly ConcurrentDictionary<int, Product> Products = new();

    public Product? FindById(int id)
    {
        return Products.TryGetValue(id, out var product) ? product : null;
    }

    public Product AddProduct(Product product)
    {
#pragma warning disable SA1101
        var productToAdd = product with { Id = Products.Count + 1 };
#pragma warning restore SA1101
        Products.TryAdd(productToAdd.Id, productToAdd);
        return productToAdd;
    }
}