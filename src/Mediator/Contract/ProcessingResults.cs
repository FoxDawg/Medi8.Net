using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mediator.Contract;

public class ProcessingResults : ReadOnlyCollection<ProcessingResult>
{
    internal ProcessingResults()
        : base(new List<ProcessingResult>())
    {
    }

    public ProcessingResults(IEnumerable<ProcessingResult> list)
        : base(list.ToList())
    {
    }

    public ProcessingResults(ICollection<ProcessingResult> list)
        : base(list.ToList())
    {
    }

    public static ProcessingResults Empty => new();
}