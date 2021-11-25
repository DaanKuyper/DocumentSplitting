namespace WobModels;

public class Document
{
  public string? Name { get; set; }

  public string? Href { get; set; }

  public string? Url { get; set; }

  public string? FileType { get; set; }

  public string? Pages { get; set; }

  public string? DocumentType { get; set; }

  public string? Date { get; set; }


  public int PageCount => int.TryParse(Pages, out int result) ? result : 0;

  public override string ToString() => Name ?? "Únknown";
}
