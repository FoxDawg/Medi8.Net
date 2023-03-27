using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mediator.Contract;

public class Errors : ReadOnlyCollection<Error>
{
    internal Errors()
        : base(new List<Error>())
    {
    }

    public Errors(IEnumerable<Error> list)
        : base(list.ToList())
    {
    }

    public Errors(ICollection<Error> list)
        : base(list.ToList())
    {
    }

    public static Errors Empty => new();
}