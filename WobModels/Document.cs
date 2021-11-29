
namespace WobModels;

public class Document
{
  public Document()
  {
  }

  public Document(string wobUrl, string matchString)
  {
    var regex = new Regex("<span>(.*?)</span>");
    Name = regex.Match(matchString).Groups[1].Value;

    regex = new Regex("href=\"(.*?)\"");
    Href = regex.Match(matchString).Groups[1].Value;

    Url = $"{wobUrl}{Href}";

    regex = new Regex("<div class=\"meta__line\">(.*?)</div>");

    var metaData = regex.Matches(matchString);
    var metaElements = metaData[0].Groups[1].Value.Split('|');

    FileType = TrimHtml(metaElements[0]);
    Pages = TrimHtml(metaElements[1].Replace("pagina", "").Replace("'s", ""));
    Size = TrimHtml(metaElements[2]);

    metaElements = metaData[1].Groups[1].Value.Split('|');

    DocumentType = TrimHtml(metaElements[0]);
    Date = TrimHtml(metaElements[1]);
  }


  public string? Name { get; set; }

  public string? Href { get; set; }

  public string? Url { get; set; }

  public string? FileType { get; set; }

  public string? Pages { get; set; }

  public string? Size { get; set; }

  public string? DocumentType { get; set; }

  public string? Date { get; set; }


  public int MetaPageCount => int.TryParse(Pages, out int result) ? result : 0;


  public override string ToString() => Name ?? "Únknown";

  private static string TrimHtml(string input)
    => input.Replace("&nbsp;", "").Trim();
}
