namespace SplittingComponents;

public class ReportController
{
  public static async Task WriteMetaDataReport(WobFileList wobFiles, string reportFile, HttpController httpControl)
  {
    if (string.IsNullOrEmpty(reportFile))
    {
      // TODO prettify...
      throw new Exception("missing path for report file...");
    }

    var csv = new StringBuilder();
    csv.AppendLine(
      "WobFile, WobURL, WobTitle, WobDate," +
      "PdfName, PdfUrl, PdfDate, Size, PdfTitle, PdfPages (meta), WordsPerPage"
    );

    for (var index = 0; index < wobFiles.Count; index++)
    {
      var wobFile = wobFiles[index];
      if (wobFile.Documents == null)
      {
        csv.AppendLine($"{wobFile.Id},{wobFile.Url},{wobFile.Title},{wobFile.Date},,,,,,,");
      }
      else
      {
        foreach (var document in wobFile.Documents)
        {
          using var pdf = await RetrievePdf(document, httpControl);

          // TODO desperately prettify...
          if (pdf != null)
          {
            csv.AppendLine(
              $"{wobFile.Id},{wobFile.Url},{wobFile.Title?.Replace(',', '.')},{wobFile.Date}," +
              $"{document.Name?.Replace(',', '.')},{document.Url},{document.Date},{document.Size}," +
              $"{pdf.Title?.Replace(',', '.')},{pdf.PageCount} ({document.MetaPageCount}),{pdf.AverageWordsPerPage}"
            );
          }
          else
          {
            csv.AppendLine(
              $"{wobFile.Id},{wobFile.Url},{wobFile.Title?.Replace(',', '.')},{wobFile.Date}," +
              $"{document.Name?.Replace(',', '.')},{document.Url},{document.Date},{document.Size}," +
              $"ERROR,0 ({document.MetaPageCount}),0"
            );
          }
        }
      }
      Console.WriteLine($"[{index}] - {wobFile.Id} Passed");
    }

    File.WriteAllText(reportFile, csv.ToString());
  }


  public static async Task<PdfDocumentClass> RetrievePdf(Document document, HttpController httpControl)
  {
    if (string.IsNullOrWhiteSpace(document.Url))
    {
      // TODO prettify...
      throw new Exception($"missing Url : {document}");
    }

    try
    {
      using var stream = await httpControl.RetrievePdfStream(document.Url);
      using var pdf = new PdfDocumentClass(stream);

      return pdf;
    }
    // TODO prettify...
    catch (Exception ex)
    {
      return null;
    }
  }

}

