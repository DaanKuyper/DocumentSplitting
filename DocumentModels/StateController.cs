namespace SplittingComponents;

public class StateController
{
  public StateController(string configFile)
  {
    Config = Configuration.Load(configFile);

    LogControl = new LogController(Config.OutputLogFile);
    HttpControl = new HttpController(Config.BaseUrl, Config.ApiUrl);

    LogControl.Write("Successfully initialized...");
  }


  public async Task RetrieveMeta()
  {
    WobFiles = await RetrieveWobFiles();
    WriteJsonWobFiles();
  }


  public async Task<WobFileList> RetrieveWobFiles()
  {
    var operationName = "Retrieving WOB files";
    LogControl.StartOperation(operationName);

    var result = new WobFileList();

    if (string.IsNullOrEmpty(Config.ApiUrl))
    {
      HandleException(new ConfigException("missing API url..."));
    }

    var jsonString = await HttpControl.RetrieveWobList();
    var data = JsonSerializer.Deserialize<ApiResultDataList>(jsonString);

    if (data == null || data.IsEmpty)
    {
      HandleException(new DataException("missing documents in apiresult..."));
    }

    foreach (var record in data.results)
    {
      LogControl.IterationPassed(operationName);

      var url = HttpControl.WobDocumentUrl(record.Id);
      var htmlString = await HttpControl.RetrieveWobDocument(url);

      var documents = new DocumentList(url, htmlString);
      var wobFile = new WobFile(record, documents, url);

      result.Add(wobFile);
    }

    LogControl.FinishOperation(operationName);
    return result;
  }


  public WobFileList ReadJsonWobFiles()
  {
    var result = new WobFileList();

    if (string.IsNullOrEmpty(Config.LocalMetaStoragePath))
    {
      HandleException(new ConfigException("missing json path in configuration..."));
    }

    var fileNames = Directory.GetFiles(Config.LocalMetaStoragePath);
    if (fileNames.Length == 0)
    {
      HandleException(new DataException("missing wob files in local meta storage path..."));
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
      catch (Exception ex)
      {
        HandleException(new DataException($"json issue encountered in `{fileName}`: {ex.Message}"), false);
      }
    }
    return result;
  }


  public void WriteJsonWobFiles()
  {
    if (string.IsNullOrEmpty(Config.LocalMetaStoragePath))
    {
      HandleException(new ConfigException("missing json path in configuration..."));
    }

    foreach (var wobFile in WobFiles)
    {
      var jsonString = JsonSerializer.Serialize(wobFile);
      var filePath = StringExtensions.FilePath(Config.LocalMetaStoragePath, wobFile.FileName);

      File.WriteAllText(filePath, jsonString);
    }
  }


  public async Task RetrievePdfs()
  {
    if (WobFiles.IsEmpty)
    {
      WobFiles = Config.UseLocalWobMetaData ?
        ReadJsonWobFiles() : await RetrieveWobFiles();
    }

    var operationName = "Retrieving PDFs";
    LogControl.StartOperation(operationName);

    foreach (var wobFile in WobFiles)
    {
      foreach (var document in wobFile.Documents)
      {
        LogControl.IterationPassed(operationName);

        await RetrievePdf(document, wobFile.DocumentFileName(document));
      }
    }

    LogControl.FinishOperation(operationName);
  }


  public async Task RetrievePdf(Document document, string documentName)
  {
    if (string.IsNullOrWhiteSpace(document.Url))
    {
      HandleException(new DataException($"missing Url in document : `{document}`"), false);
      return;
    }

    try
    {
      var filePath = StringExtensions.FilePath(Config.LocalPdfStoragePath, documentName);

      using var pdfStream = await HttpControl.RetrievePdfStream(document.Url);
      using var fileStream = File.Create(filePath);

      pdfStream.CopyTo(fileStream);
    }
    // TODO prettify...
    catch (Exception ex)
    {
      HandleException(new DataException(ex.Message), false);
    }
  }


  public void HandleException(Exception exception, bool isBreaking = true)
  {
    LogControl.Write(exception.Source ?? string.Empty, exception.Message);
    if (isBreaking)
    {
      throw exception;
    }
  }


  public void WriteMetaDataReport()
  {
    try
    {
      ReportController.WriteMetaDataReport(
        WobFiles, Config.LocalMetaDataReportFile, LogControl);
    }
    catch (Exception ex)
    {
      HandleException(ex);
    }
  }


  public void WritePdfDataReport()
  {
    try
    {
      // TODO : possibility to write report on remote Pdfs...
      ReportController.WritePdfOverviewReport(
        Config.LocalPdfStoragePath, Config.LocalPdfReportFile, LogControl);
    }
    catch (Exception ex)
    {
      HandleException(ex);
    }
  }


  public WobFileList WobFiles { get; set; } = new();


  readonly Configuration Config;

  readonly LogController LogControl;

  readonly HttpController HttpControl;
}
