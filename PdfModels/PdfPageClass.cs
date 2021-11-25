namespace PdfModels;

public class PdfPageClass
{
  public PdfPageClass(PdfPage page)
  {
    Page = page;

    var strategy = new SimpleTextExtractionStrategy();

    Content = PdfTextExtractor.GetTextFromPage(Page, strategy);
    WordCount = Content.Split().Length;
  }


  public bool IsScan => string.IsNullOrWhiteSpace(Content);

  public string Content { get; set; } = string.Empty;

  public int WordCount { get; set; } = 0;


  private readonly PdfPage Page;
}
