namespace Sample.WebApi.Products.Model;

public record Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}