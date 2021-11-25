namespace SplittingComponents;

public class HttpException : Exception
{
  public HttpException(string message, Exception? innerException)
    : base(message, innerException)
  {
  }

  public HttpException(string url, string? innerExceptionMessage)
    : this($"URL not found: {url}", new HttpRequestException(innerExceptionMessage))
  {
  }
}
