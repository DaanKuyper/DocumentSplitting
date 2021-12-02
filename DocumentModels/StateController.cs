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


  public async Task RetrievePdfs(bool overwriteExisting = true)
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
        var documentFileName = wobFile.DocumentFileName(document);
        await RetrievePdf(document, documentFileName, overwriteExisting);

        LogControl.IterationPassed(operationName, $"{wobFile.Id}-{document.Name}");
      }
    }

    LogControl.FinishOperation(operationName);
  }


  public async Task RetrievePdf(
    Document document, string fileName, bool overwriteExisting)
  {
    if (string.IsNullOrWhiteSpace(document.Url))
    {
      HandleException(new DataException($"missing Url in document : `{document}`"), false);
      return;
    }

    try
    {
      var filePath = StringExtensions.FilePath(Config.LocalPdfStoragePath, fileName);
      if (overwriteExisting || !File.Exists(filePath))
      {
        using var pdfStream = await HttpControl.RetrievePdfStream(document.Url);
        using var fileStream = File.Create(filePath);

        pdfStream.CopyTo(fileStream);
      }
    }
    // TODO prettify...
    catch (Exception ex)
    {
      HandleException(new DataException(ex.Message), false);
    }
  }


  public void ConvertPdfsToHtml(bool overwriteExisting)
  {
    var operationName = "Converting Pdfs to Html";
    LogControl.StartOperation(operationName);

    var filePaths = Directory.GetFiles(Config.LocalPdfStoragePath, "*.pdf");
    if (filePaths.Length == 0)
    {
      throw new DataException("missing files in local meta storage path...");
    }

    foreach (var filePath in filePaths)
    {
      var fileName = Path.GetFileNameWithoutExtension(filePath);

      ConvertPdfToHtml(filePath, fileName, overwriteExisting);
      LogControl.IterationPassed(operationName, fileName);
    }

    LogControl.FinishOperation(operationName);
  }


  public void ConvertPdfToHtml(
    string filePath, string fileName, bool overwriteExisting)
  {
    try
    {
      XPdfController.PdfToHtml(
        filePath, Config.LocalHtmlStoragePath, fileName, overwriteExisting, LogControl);
    }
    catch(Exception ex)
    {
      HandleException(ex);
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


  public void WriteOcrReport()
  {
    try
    {
      // TODO : possibility to write report on remote Pdfs...
      ReportController.WriteOcrReport(
        Config.LocalPdfStoragePath, Config.LocalHtmlStoragePath, 
        Config.LocalOcrStoragePath,Config.LocalOcrReportFile, LogControl);
    }
    catch (Exception ex)
    {
      HandleException(ex);
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


  public WobFileList WobFiles { get; set; } = new();


  readonly Configuration Config;

  readonly LogController LogControl;

  readonly HttpController HttpControl;
}
