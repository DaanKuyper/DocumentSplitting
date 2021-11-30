namespace PdfModels.iText;

public class PdfDocumentClass : IDisposable
{
  public PdfDocumentClass()
  {
    using var tempStream = new MemoryStream();

    document = new PdfDocument(new PdfWriter(tempStream));
    documentInfo = document.GetDocumentInfo();
  }

  public PdfDocumentClass(Stream stream)
  {
    document = new PdfDocument(new PdfReader(stream));
    documentInfo = document.GetDocumentInfo();

    for (int i = 1; i <= document.GetNumberOfPages(); ++i)
    {
      pages.Add(new PdfPageClass(document.GetPage(i)));
    }
  }

  public List<PdfPageClass> GetPages() => pages;



  public string Title => documentInfo.GetTitle();

  public string Author => documentInfo.GetAuthor();

  public string Subject => documentInfo.GetSubject();

  public string Keywords => documentInfo.GetKeywords();

  public string Creator => documentInfo.GetCreator();

  public string Producer => documentInfo.GetProducer();

  public string CreationDate => documentInfo.GetMoreInfo("CreationDate");

  public string ModDate => documentInfo.GetMoreInfo("ModDate");

  public string Language => document.GetCatalog().GetLang()?.ToString() ?? string.Empty;

  public string PdfVersion => document.GetPdfVersion()?.ToString() ?? string.Empty;

  public string FileSize => $"{document.GetReader().GetFileLength()} bytes";


  private List<PdfPageClass> pages { get; set; } = new();

  public int PageCount => pages.Count;

  public int WordCount => pages.Sum(page => page.WordCount);

  public float AverageWordsPerPage
    => PageCount > 0 ? WordCount / PageCount : 0;



  private readonly PdfDocument document;

  private readonly PdfDocumentInfo documentInfo;

  public void Dispose() => document.Close();
}