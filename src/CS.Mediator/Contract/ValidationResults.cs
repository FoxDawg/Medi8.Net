using System.Collections.ObjectModel;

namespace CS.Mediator.Contract;

public class ValidationResults : ReadOnlyCollection<ValidationResult>
{
    internal ValidationResults()
        : base(new List<ValidationResult>())
    {
    }

    public ValidationResults(IEnumerable<ValidationResult> list)
        : base(list.ToList())
    {
    }

    public ValidationResults(List<ValidationResult> list)
        : base(list)
    {
    }

    public static ValidationResults Empty => new();
}