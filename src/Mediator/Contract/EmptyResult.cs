namespace Mediator.Contract;

public record EmptyResult
{
    public static EmptyResult Create => new ();
}