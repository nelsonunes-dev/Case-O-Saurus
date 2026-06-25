namespace CaseOSaurus.Application.Common.Exceptions;

public class ConcurrencyException : Exception
{
    public ConcurrencyException(string message, Exception? innerException = null) : base(message, innerException) { }

    public ConcurrencyException() { }

    public ConcurrencyException(string message) : base(message) { }
}
