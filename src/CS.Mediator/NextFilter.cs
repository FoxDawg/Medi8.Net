using System.Threading.Tasks;

namespace CS.Mediator;

public delegate Task NextFilter(ProcessingContext context);