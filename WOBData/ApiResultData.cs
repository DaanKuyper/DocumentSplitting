namespace WobModels;

public class ApiResultData
{
  public string Id => link?.Replace("#", string.Empty) ?? string.Empty;

  public string Summary => publication?.summary ?? string.Empty;


  public string? link { get; set; }

  public Publication? publication { get; set; }

  public class Publication
  {
    public string? id { get; set; }

    public string? summary { get; set; }

    public DateTime publicationdata { get; set; }
  }
}
