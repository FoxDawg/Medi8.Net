namespace CS.Mediator.Contract;

public enum StatusCode
{
    None,
    Ok = 200,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    InternalError = 500,
    PipelineFailed = 600,
}