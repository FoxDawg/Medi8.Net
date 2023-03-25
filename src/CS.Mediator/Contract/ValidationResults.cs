using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

    public ValidationResults(ICollection<ValidationResult> list)
        : base(list.ToList())
    {
    }

    public static ValidationResults Empty => new();
}