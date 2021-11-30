namespace SplittingComponents;

public class ReportController
{
  public static void WriteMetaDataReport(
    WobFileList wobFiles, string reportFile, LogController logControl)
  {
    var operationName = "Writing MetaData Report";
    logControl.StartOperation(operationName);

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


  public static void WritePdfReport(string pdfLocation, string reportFile, LogController logControl)
  {
    var operationName = "Writing Pdf Report";
    logControl.StartOperation(operationName);

    // TODO : ...

    File.WriteAllText(reportFile, string.Empty);
    logControl.FinishOperation(operationName);
  }
}

