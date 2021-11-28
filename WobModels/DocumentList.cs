namespace WobModels;

public class DocumentList : List<Document>
{
  public DocumentList()
  {
  }

  public DocumentList(string url, string htmlString)
  {
    // TODO prettify...
    var regex = new Regex("<a class=\"publication__download\" (.*?)/a>");
    var matches = regex.Matches(htmlString);

    foreach (Match match in matches)
    {
      Add(new Document(url, match.Value));
    }
  }
}

