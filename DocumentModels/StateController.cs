namespace SplittingComponents;

public class StateController
{
  public StateController(string configFile)
  {
    Config = Configuration.Load(configFile);

    HttpControl = new HttpController(Config.BaseUrl, Config.ApiUrl);

    if (Config.RetrieveOnStart)
    {
      WobFiles = Task.Run(RetrieveWobFiles).Result;
      WriteJsonWobFiles();
    }
    else
    {
      WobFiles = ReadJsonWobFiles();
    }
  }


  public async Task<WobFileList> RetrieveWobFiles()
  {
    var result = new WobFileList(HttpControl);

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
      var url = HttpControl.WobDocumentUrl(record.Id);

      result.Add(new WobFile(record, await RetrieveDocuments(url), url));
    }

    return result;
  }

  public async Task<List<Document>> RetrieveDocuments(string url)
  {
    var result = new List<Document>();

    var htmlString = await HttpControl.RetrieveWobDocument(url);

    // TODO prettify...
    var regex = new Regex("<a class=\"publication__download\" (.*?)/a>");
    var matches = regex.Matches(htmlString);

    foreach (Match match in matches)
    {
      result.Add(new Document(url, match.Value));
    }

    return result;
  }


  public WobFileList ReadJsonWobFiles()
  {
    var result = new WobFileList(HttpControl);

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

  
  public void WriteJsonWobFiles()
  {
    if (string.IsNullOrEmpty(Config.LocalStoragePath))
    {
      // TODO prettify...
      throw new Exception("missing json path in configuration...");
    }

    foreach (var wobFile in WobFiles)
    {
      var jsonString = JsonSerializer.Serialize(wobFile);
      var path = $"{Config.LocalStoragePath}{Path.DirectorySeparatorChar}{wobFile.FileName}";

      File.WriteAllText(path, jsonString);
    }
  }


  public async Task ParseWobFileList() => await WobFiles.Parse();


  public WobFileList WobFiles { get; set; }


  readonly Configuration Config;

  readonly HttpController HttpControl;
}
