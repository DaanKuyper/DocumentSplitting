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
      throw new DataException("missing files in local meta storage path...");
    }

    //TODO !!!
    //var properties = typeof(PdfDocumentInfo).GetProperties();
    var properties = typeof(PdfDocumentInfo).GetProperties();

    var csv = new StringBuilder();

    csv.Append("FileName,");
    csv.AppendLine(CsvHeader(properties));

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


  //public static void WritePdfReport(
  //  string pdfFile, string reportFile, LogController logControl)
  //{
  //  var operationName = "Writing Pdf Report";
  //  logControl.StartOperation(operationName);


  //  var pdfProps = typeof(PdfDocumentClass).GetProperties();
  //  var ocrProps = typeof(OcrClass).GetProperties();

  //  var csv = new StringBuilder();
  //  csv.AppendLine($"{CsvHeader(pdfProps)},{CsvHeader(ocrProps)}");

  //  try
  //  {
  //    using var pdf = new PdfDocumentClass(File.OpenRead(pdfFile));
  //    using var ocr = new OcrClass();

  //    csv.Append(CsvValues(pdfProps, pdf));


  //    for (var i = 0; i < ocrProps.Length; i++)
  //    {

  //    }

  //  }
  //  catch (Exception ex)
  //  {
  //    logControl.WriteError(ex.Message, filePath);
  //  }


  //  File.WriteAllText(reportFile, csv.ToString());
  //  logControl.FinishOperation(operationName);
  //}


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

