using System.Net;

namespace server.Classes;

public class DomainException : Exception
{
    public HttpStatusCode ErrorCode { get; }

    public DomainException(HttpStatusCode errorCode, string errorMessage) : base(errorMessage)
    {
        ErrorCode = errorCode;
    }
}
