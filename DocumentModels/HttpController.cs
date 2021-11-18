namespace SplittingComponents;

public class HttpController
{
  public async Task<PdfClass> RetrievePdf(string url)
  {
    var httpResult = await TryRetrieveHttp(url);
    if (!httpResult.IsSuccessStatusCode)
    {
      // TODO: prettify...
      throw new Exception("PDF not found...");
    }

    using var stream = await httpResult.Content.ReadAsStreamAsync();
    if (httpResult.Content.Headers.ContentType == null)
    {
      // TODO: prettify...
      throw new Exception("Content type is not set...");
    }

    return httpResult.Content.Headers.ContentType.MediaType switch
    {
      "application/pdf" => new PdfClass(stream, url, httpResult.StatusCode),

      // TODO: prettify...
      _ => throw new Exception("Content type is not PDF...")
    };
  }

  public async Task<HttpResponseMessage> TryRetrieveHttp(string url)
  {
    try
    {
      return await httpClient.GetAsync(url);
    }
    catch (Exception ex)
    {
      if (ex is HttpRequestException || ex is InvalidOperationException)
      {
        return new HttpResponseMessage(HttpStatusCode.BadGateway)
        {
          ReasonPhrase = ex.Message
        };
      }
      throw;
    }
  }

  private readonly HttpClient httpClient = new();
}
