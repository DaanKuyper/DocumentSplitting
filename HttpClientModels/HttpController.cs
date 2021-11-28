using PdfModels;

namespace HttpClientModels;

public class HttpController
{
  public HttpController(string baseUrl, string apiUrl)
  {
    BaseUrl = baseUrl;
    ApiUrl = apiUrl;
  }


  public async Task<string> RetrieveWobList()
  {
    var (httpResult, mediaType) = await TryRetrieveHttp(ApiUrl);

    if (mediaType != "application/json")
    {
      throw new HttpContentException("JSON", ApiUrl, mediaType);
    }

    return await httpResult.Content.ReadAsStringAsync();
  }

  
  public async Task<string> RetrieveWobDocument(string url)
  {
    var (httpResult, mediaType) = await TryRetrieveHttp(url);

    if (mediaType != "text/html")
    {
      throw new HttpContentException("HTML", ApiUrl, mediaType);
    }

    return await httpResult.Content.ReadAsStringAsync();
  }


  public async Task<Stream> RetrievePdfStream(string url)
  {
    var (httpResult, mediaType) = await TryRetrieveHttp(url, true);

    if (mediaType != "application/pdf")
    {
      throw new HttpContentException("PDF", url, mediaType);
    }

    return await httpResult.Content.ReadAsStreamAsync();
  }


  public async Task<(HttpResponseMessage, string?)> TryRetrieveHttp(string url, bool headersOnly = false)
  {
    try
    {
      var httpResult = headersOnly ? 
        await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead) :
        await httpClient.GetAsync(url);

      if (!httpResult.IsSuccessStatusCode)
      {
        throw new HttpException(url, httpResult.ReasonPhrase);
      }

      if (httpResult.Content.Headers.ContentType == null)
      {
        throw new HttpContentException(url);
      }

      return (httpResult, httpResult.Content.Headers.ContentType.MediaType);
    }
    catch (Exception ex)
    {
      if (ex is HttpRequestException || ex is InvalidOperationException)
      {
        throw new HttpException(url, ex.Message);
      }
      throw;
    }
  }


  public string WobDocumentUrl(string id) => $"{BaseUrl}/publicaties/{id}/";


  private readonly string BaseUrl;

  private readonly string ApiUrl;

  private readonly HttpClient httpClient = new();
}
