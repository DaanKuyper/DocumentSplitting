namespace PdfModels;

public class PdfDocumentClass : IDisposable
{
  public PdfDocumentClass()
  {
    document = new PdfDocument();
  }

  public PdfDocumentClass(Stream stream)
  {
    try
    {
      document = PdfReader.Open(stream, PdfDocumentOpenMode.ReadOnly);
      foreach (var page in document.Pages)
      {
        pages.Add(new PdfPageClass(page));
      }
    }
    catch (PdfReaderException ex)
    {
      document = new PdfDocument();

      document.Info.Title = ex.Message;
    }
  }


  public string Title => document.Info.Title;

  public string Author => document.Info.Author;

  public string Subject => document.Info.Subject;

  public string Keywords => document.Info.Keywords;

  public string Creator => document.Info.Creator;

  public string Producer => document.Info.Producer;

  public string CreationDate => document.Info.CreationDate.ToString();

  public string ModData => document.Info.ModificationDate.ToString();


  public string Language => document.Internals.Catalog.Language;

  public string PdfVersion => document.Internals.Catalog.Version;

  public string FileSize => $"{document.FileSize} bytes";



  private readonly List<PdfPageClass> pages = new();

  public int PageCount => document.PageCount;

  public int WordCount => pages.Sum(page => page.WordCount);

  public float AverageWordsPerPage
    => PageCount > 0 ? WordCount / PageCount : 0;



  private readonly PdfDocument document;

  public void Dispose() => document.Close();
}
