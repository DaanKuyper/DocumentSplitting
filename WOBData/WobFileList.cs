namespace WobModels;

public class WobFileList : List<WobFile>
{
  public WobFileList(HttpController httpControl)
  {
    HttpControl = httpControl;
  }


  public async Task Parse()
  {
    foreach (var wobFile in this)
    {
      await ParseWobFile(wobFile);
    }
  }

  public async Task ParseWobFile(WobFile wobFile)
  {
    if (string.IsNullOrWhiteSpace(wobFile.Url))
    {
      // TODO prettify...
      throw new Exception($"missing Url : {wobFile}");
    }

    if (wobFile.Documents == null || wobFile.Documents.Count == 0)
    {
      // TODO prettify...
      throw new Exception("missing wob documents...");
    }

    // TODO :
    //  build dictionary containing PDF documents regarding same subject /  other grouping methods...
    foreach (var document in wobFile.Documents)
    {
      var pdf = await ParsePdf(document);
    }
  }

  public async Task<PdfDocumentClass> ParsePdf(Document document)
  {
    if (string.IsNullOrWhiteSpace(document.Url))
    {
      // TODO prettify...
      throw new Exception($"missing Url : {document}");
    }

    return await HttpControl.RetrievePdf(document.Url);
  }


  public void AddFile(WobFile? wobFile)
  {
    // TODO prettify...
    if (wobFile == null)
    {
      throw new Exception();
    }
    Add(wobFile);
  }



  private readonly HttpController HttpControl;
}

