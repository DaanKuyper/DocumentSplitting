namespace WobModels;

public class WobFile
{
  public string? Id { get; set; }

  public string? Url { get; set; }

  public string? Title { get; set; }

  public string? Summary { get; set; }

  public string? Date { get; set; }

  public string? FileName { get; set; }


  public List<Document>? Documents { get; set; }

  public override string ToString() => FileName ?? Id ?? "Unknown";
}
