namespace SplittingComponents;

public class StateController
{
  public StateController(string configFile)
  {
    Config = Configuration.Load(configFile);

    HttpControl = new HttpController(Config.BaseUrl, Config.ApiUrl);

    if (Config.RetrieveWobFiles)
    {
      WobFiles = Task.Run(RetrieveWobFiles).Result;
    }
    else
    {
      WobFiles = ParseJsonWobFiles();
    }
  }


  public async Task<WobFileList> RetrieveWobFiles()
  {
    var result = new WobFileList();

    if (string.IsNullOrEmpty(Config.ApiUrl))
    {
      // TODO prettify...
      throw new Exception("missing API url in configuration...");
    }

    var jsonString = await HttpControl.RetrieveWobList();
    var data = JsonSerializer.Deserialize<ApiResultDataList>(jsonString);

    // TODO prettify...
    if (data == null)
    {
      throw new Exception();
    }

    // TODO prettify...
    if (data.results == null || data.results.Count == 0)
    {
      throw new Exception();
    }

    foreach (var record in data.results)
    {
      var document = await HttpControl.RetrieveWobDocument(record.Id);

    }

    return result;
  }


  public WobFileList ParseJsonWobFiles()
  {
    var result = new WobFileList();

    if (string.IsNullOrEmpty(Config.LocalStoragePath))
    {
      // TODO prettify...
      throw new Exception("missing json path in configuration...");
    }

    var fileNames = Directory.GetFiles(Config.LocalStoragePath);
    if (fileNames.Length == 0)
    {
      // TODO prettify...
      throw new Exception("missing wob files...");
    }

    foreach (var fileName in fileNames)
    {
      try
      {
        var jsonString = File.ReadAllText(fileName);
        var wobFile = JsonSerializer.Deserialize<WobFile>(jsonString);

        result.AddFile(wobFile);
      }
      // TODO prettify...
      catch (Exception)
      {
        throw new Exception($"Json issue encountered: `{fileName}`");
      }
    }
    return result;
  }


  public async Task ParseWob(WobFile wobFile)
  {
    if (string.IsNullOrWhiteSpace(wobFile.Url))
    {
      // TODO prettify...
      throw new Exception($"missing Url : {wobFile}");
    }

    if (wobFile.Documents == null || wobFile.Documents.Count == 0)
    {
      // TODO prettify...
      throw new Exception("missing wob documents...");
    }

    // TODO :
    //  build dictionary containing PDF documents regarding same subject /  other grouping methods...
    foreach (var document in wobFile.Documents)
    {
      var pdf = await ParsePdf(document);
    }
  }

  public async Task<PdfClass> ParsePdf(Document document)
  {
    if (string.IsNullOrWhiteSpace(document.Url))
    {
      // TODO prettify...
      throw new Exception($"missing Url : {document}");
    }

    return await HttpControl.RetrievePdf(document.Url);
  }


  public WobFileList WobFiles { get; set; } = new();


  readonly Configuration Config;

  readonly HttpController HttpControl;
}
