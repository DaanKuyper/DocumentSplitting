namespace SplittingComponents;

public class HttpContentException : Exception
{
  public HttpContentException(string url)
    : base($"Content type is not set: {url}")
  {
  }

  public HttpContentException(string expectedType, string url, string? type)
    : base($"Unexpected type '{type}' for {expectedType}: {url}")
  {
  }
}

