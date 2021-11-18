using System.Net;
using System.Text;

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace PdfModels;

public class PdfClass
{
  public PdfClass(Stream stream)
  {
    document = new PdfDocument(new PdfReader(stream));
    documentInfo = document.GetDocumentInfo();

    ParsePages();
  }

  public PdfClass(Stream stream, string url, HttpStatusCode statusCode) : this(stream)
  {
    Url = url;
    HttpStatus = statusCode;
  }


  void ParsePages()
  {
    var sb = new StringBuilder();

    for (int i = 1; i <= document.GetNumberOfPages(); ++i)
    {
      var page = document.GetPage(i);
      var text = PdfTextExtractor.GetTextFromPage(page);

      sb.Append(text);
      sb.AppendLine();
    }

    // TODO : 
    // - Keep track of text relative to page..
    // - Search pages for keywords to determine type of Page (and document).
    // - Make overview of different document types and their subjects based on text found in PDF...

    var result = sb.ToString();
  }


  public string Title => documentInfo.GetTitle();

  public string Author => documentInfo.GetAuthor();

  public string Subject => documentInfo.GetSubject();

  public string Keywords => documentInfo.GetKeywords();


  public string? Url { get; }

  public HttpStatusCode HttpStatus { get; }


  readonly PdfDocument document;

  readonly PdfDocumentInfo documentInfo;
}
