namespace Mediator.Contract;

public abstract record Status(int Value)
{
    public static OkStatus Ok { get; } = new();
    public static PipelineFailedStatus PipelineFailed { get; } = new();
    public static CancellationRequestedStatus CancellationRequested { get; } = new();
    public static ValidationFailedStatus ValidationFailed { get; } = new();

    public record OkStatus() : Status(20);

    public record PipelineFailedStatus() : Status(30);

    public record CancellationRequestedStatus() : Status(40);

    public record ValidationFailedStatus() : Status(60);
}