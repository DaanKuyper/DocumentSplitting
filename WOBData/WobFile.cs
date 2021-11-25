namespace WobModels;

public class WobFile
{
  public WobFile()
  {
  }

  public WobFile(ApiResultData record, List<Document> documents, string url)
  {
    Id = record.Id;
    Title = record.Title;
    Summary = record.Summary;
    Date = record.Date;

    FileName = $"{record.Date:yyyy-MM-dd}_{Id}.json";

    Documents = documents;
    Url = url;
  }

  public string? Id { get; set; }

  public string? Url { get; set; }

  public string? Title { get; set; }

  public string? Summary { get; set; }

  public DateTime? Date { get; set; }

  public string? FileName { get; set; }


  public List<Document>? Documents { get; set; }


  public override string ToString() => FileName ?? Id ?? "Unknown";
}
