using System.Text.RegularExpressions;

namespace PdfModels;

public class PdfDocumentInfo
{
  public PdfDocumentInfo(string input, string error)
  {
    Title = FindValue(input, "Title");
    Subject = FindValue(input, "Subject");
    Keywords = FindValue(input, "Keywords");
    Author = FindValue(input, "Author");
    Creator = FindValue(input, "Creator");
    Producer = FindValue(input, "Producer");
    CreationDate = FindValue(input, "CreationDate");
    ModificationDate = FindValue(input, "ModDate");

    Tagged = FindValue(input, "Tagged");
    Form = FindValue(input, "Form");
    PageCount = FindValue(input, "Pages");
    Encrypted = FindValue(input, "Encrypted");
    Permissions = FindValue(input, "Permissions");
    PageSize = FindValue(input, "Page size");
    FileSize = FindValue(input, "File size");
    Optimized = FindValue(input, "Optimized");
    PdfVersion = FindValue(input, "PDF version");

    ErrorMessage = error;
  }

  private static string FindValue(string input, string searchValue)
  {
    var regex = new Regex($"{searchValue}:(.*?)\n", RegexOptions.IgnoreCase);
    var match = regex.Match(input);
    if (match.Groups.Count == 2)
    {
      return match.Groups[1].Value.Trim();
    }
    return string.Empty;
  }


  public string Title { get; set; }

  public string Subject { get; set; }

  public string Keywords { get; set; }

  public string Author { get; set; }

  public string Creator { get; set; }

  public string Producer { get; set; }

  public string CreationDate { get; set; }

  public string ModificationDate { get; set; }


  public string Tagged { get; set; }

  public string Form { get; set; }

  public string PageCount { get; set; }

  public string Encrypted { get; set; }

  public string Permissions { get; set; }

  public string PageSize { get; set; }

  public string FileSize { get; set; }

  public string Optimized { get; set; }

  public string PdfVersion { get; set; }


  public string ErrorMessage { get; set; }
}

