using System.Threading.Tasks;

namespace Mediator.Pipeline;

public delegate Task NextFilter(ProcessingContext context);