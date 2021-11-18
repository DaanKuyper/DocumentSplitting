namespace SplittingComponents;

public class StateController
{
  public StateController(string configFile)
  {
    Config = Configuration.Load(configFile);

    HttpControl = new HttpController();

    WobFiles = Parse(Config.JsonPath);

  }
  

  public static WobFileList Parse(string? jsonPath)
  {
    var result = new WobFileList();

    if (string.IsNullOrEmpty(jsonPath))
    {
      // TODO prettify...
      throw new Exception("missing json path in configuration...");
    }

    var fileNames = Directory.GetFiles(jsonPath);
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

      pdf.IteratePages();
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
