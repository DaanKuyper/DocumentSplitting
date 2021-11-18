using System.Net;
using System.Text;

using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.Advanced;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Pdf.Content;
using PdfSharpCore.Pdf.Content.Objects;

namespace PdfModels;

public class PdfClass
{
  public PdfClass(Stream stream)
  {
    pdfDocument = PdfReader.Open(stream, PdfDocumentOpenMode.ReadOnly);
  }

  public PdfClass(Stream stream, string url, HttpStatusCode statusCode)
  {
    Url = url;
    HttpStatus = statusCode;

    //pdfDocument = PdfReader.Open(stream, PdfDocumentOpenMode.ReadOnly);


    // TEMP TEST:
    var path = @"C:\Users\daan_\source\repos\DocumentSplitting\Literature\besluit-op-wob-verzoek-over-contacten-bedrijfsleven-coronasteunmaatregelen.pdf";
    using (var _document = PdfReader.Open(path, PdfDocumentOpenMode.ReadOnly))
    {
      var result = new StringBuilder();

      var page = _document.Pages[0];
      ExtractText(ContentReader.ReadContent(page), result);
      result.AppendLine();

      var text = result.ToString();
    }
  }

  
  public void IteratePages()
  {
    // TODO make better...
    if (Pages == null)
    {
      throw new Exception();
    }

    foreach (var page in pdfDocument.Pages.OfType<PdfPage>())
    {
      var cSequence = ContentReader.ReadContent(page);

      // TODO : hier wordt het interessant...
      // https://github.com/DavidS/PdfTextract/blob/master/PdfTextract/PdfTextExtractor.cs
      
      // TODO;
      // String values gaan verloren met deze methode...
      // probeer lokale FILE stream uit te pakken ipv HttpStream.
      // 

      var result = new StringBuilder();
      foreach (var cObject in cSequence)
      {
        ExtractText(cObject, result);
        result.AppendLine();
      }
      var text = result.ToString();
    }
  }


  // BEGIN STOLEN REGION...
  void ExtractText(CObject cObject, StringBuilder result)
  {
    if (cObject is COperator)
    {
      var cOperator = cObject as COperator;
      if (cOperator.OpCode.Name == OpCodeName.Tf.ToString())
      {
        var font = 
      }
      else if (cOperator.OpCode.Name == OpCodeName.Tj.ToString() ||
               cOperator.OpCode.Name == OpCodeName.TJ.ToString())
      {
        foreach (var cOperand in cOperator.Operands)
        {
          ExtractText(cOperand, result);
        }
      }
    }
    else if (cObject is CSequence)
    {
      var cSequence = cObject as CSequence;
      foreach (var element in cSequence)
      {
        ExtractText(element, result);
      }
    }
    else if (cObject is CString)
    {
      var cString = cObject as CString;
      result.Append(cString.Value);
    }
  }
  // END REGION...


  public string Title => pdfDocument.Info.Title;
  
  public string Subject => pdfDocument.Info.Subject;

  public string Author => pdfDocument.Info.Author;

  public int PageCount => pdfDocument.PageCount;


  public PdfPages Pages => pdfDocument.Pages;


  public string? Url { get; }

  public HttpStatusCode HttpStatus { get; }


  readonly PdfDocument pdfDocument;
}
