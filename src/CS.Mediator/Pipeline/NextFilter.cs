using System.Threading.Tasks;

namespace CS.Mediator.Pipeline;

public delegate Task NextFilter(ProcessingContext context);