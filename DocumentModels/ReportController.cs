using System.Reflection;

namespace SplittingComponents;

public class ReportController
{
  public static void WriteMetaDataReport(
    WobFileList wobFiles, string reportFile, LogController logControl)
  {
    var operationName = "Writing MetaData Report";
    logControl.StartOperation(operationName);


    // TODO : dynamic Class property loop through (like PdfReport)...
    var csv = new StringBuilder();
    csv.AppendLine(
      "WobFile, WobURL, WobTitle, WobDate," +
      "DocumentName, DocumentUrl, DocumentDate, DocumentSize"
    );

    foreach (var wobFile in wobFiles)
    {
      if (wobFile.Documents == null)
      {
        csv.AppendLine(
          $"{wobFile.Id},{wobFile.Url},{wobFile.Title},{wobFile.Date},,,,");
      }
      else
      {
        foreach (var document in wobFile.Documents)
        {
          var title = StringExtensions.CsvEncode(wobFile.Title);
          var documentName = StringExtensions.CsvEncode(document.Name);

          csv.AppendLine(
            $"{wobFile.Id},{wobFile.Url},{title},{wobFile.Date}," +
            $"{documentName},{document.Url},{document.Date},{document.Size}"
          );
        }
      }
      logControl.IterationPassed(operationName, wobFile.Id);
    }

    File.WriteAllText(reportFile, csv.ToString());
    logControl.FinishOperation(operationName);
  }


  public static void WritePdfOverviewReport(
    string localPdfStoragePath, string reportFile, LogController logControl)
  {
    var operationName = "Writing Pdf OverviewReport";
    logControl.StartOperation(operationName);

    var filePaths = Directory.GetFiles(localPdfStoragePath, "*.pdf");
    if (filePaths.Length == 0)
    {
      throw new DataException("missing files in local pdf storage path...");
    }

    var properties = typeof(PdfDocumentInfo).GetProperties();
    var csv = new StringBuilder();
    csv.Append("FileName,");

    foreach (var filePath in filePaths)
    {
      try
      {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var pdfInfo = XPdfController.PdfInfo(filePath, logControl);

        csv.Append($"{fileName},");
        csv.AppendLine(CsvValues(properties, pdfInfo));

        logControl.IterationPassed(operationName, fileName);
      }
      catch (Exception ex)
      {
        logControl.WriteError(ex.Message, filePath);
      }
    }

    File.WriteAllText(reportFile, csv.ToString());
    logControl.FinishOperation(operationName);
  }


  public static void WriteOcrReport(string localPdfStoragePath, 
    string localHtmlStoragePath, string localOcrStoragePath, 
    string reportFile, LogController logControl)
  {
    var operationName = "Writing OCR Report";
    logControl.StartOperation(operationName);

    var pdfFilePaths = Directory.GetFiles(localPdfStoragePath, "*.pdf");
    if (pdfFilePaths.Length == 0)
    {
      throw new DataException("missing files in local pdf storage path...");
    }

    var htmlDirectories = Directory.GetDirectories(localHtmlStoragePath);
    if (htmlDirectories.Length == 0)
    {
      throw new DataException("missing html directories in local html storage path...");
    }

    using var ocrControl = new OcrController();

    var csv = new StringBuilder();
    csv.Append("FileName,Pages,WordCount,WordsPerPage,WordCountOcr," +
      "WordPerPageOcr,AverageOcrConfidence");

    foreach (var filePath in pdfFilePaths)
    {
      try
      {
        var fileName = Path.GetFileNameWithoutExtension(filePath);

        // TODO : this probably shouldn't happen here...
        var path = $"{localOcrStoragePath}{Path.DirectorySeparatorChar}{fileName}";
        if (!Directory.Exists(path))
        {
          Directory.CreateDirectory(path);
        }
        var resultPath = $"{path}{Path.DirectorySeparatorChar}{fileName}.txt";

        //XPdfController.PdfToText(filePath, resultPath, logControl);

        //    csv.Append($"{fileName},");
        //    csv.AppendLine(CsvValues(properties, pdfInfo));

        //    logControl.IterationPassed(operationName, fileName);
      }
      catch (Exception ex)
      {
        logControl.WriteError(ex.Message, filePath);
      }
    }

    File.WriteAllText(reportFile, csv.ToString());
    logControl.FinishOperation(operationName);
  }


  private static string CsvHeader(PropertyInfo[] properties)
  {
    var header = new StringBuilder();
    for (var i = 0; i < properties.Length; i++)
    {
      header.Append($"{(i > 0 ? "," : "")}{properties[i].Name}");
    }
    return header.ToString();
  }


  private static string CsvValues(PropertyInfo[] properties, object? obj)
  {
    var values = new StringBuilder();
    for (var i = 0; i < properties.Length; i++)
    {
      var value = properties[i].GetValue(obj)?.ToString() ?? string.Empty;

      values.Append($"{(i > 0 ? "," : "")}" +
        $"{StringExtensions.CsvEncode(value)}");
    }
    return values.ToString();
  }
}

