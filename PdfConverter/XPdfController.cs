namespace PdfModels;

public class XPdfController
{
  public static void PdfToHtml(string filePath, string folderPath, 
    string folderName, LogController logControl)
  {
    // TODO : support Linux & MacOS for PdfToHtml xpdf command...
    var path = $"{folderPath}{Path.DirectorySeparatorChar}{folderName}";
    DeleteExistingPath(path, logControl);    

    var process = new Process();
    process.StartInfo.FileName = $"xpdf{Path.DirectorySeparatorChar}pdftohtml";
    process.StartInfo.Arguments = $"{filePath} {path}";

    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.RedirectStandardError = true;

    process.Start();

    logControl.Write(process.StandardOutput.ReadToEnd());
    logControl.WriteError(process.StandardError.ReadToEnd());

    process.WaitForExit();
  }


  static void DeleteExistingPath(string path, LogController logControl)
  {
    if (Directory.Exists(path))
    {
      logControl.Write($"Path existed `{path}` - overwriting...");
      Directory.Delete(path, true);
    }
  }


  public static PdfDocumentInfo PdfInfo(string filePath, LogController logControl)
  {
    // TODO : support Linux & MacOS for PdfInfo xpdf command...
    var process = new Process();
    process.StartInfo.FileName = $"xpdf{Path.DirectorySeparatorChar}pdfinfo";
    process.StartInfo.Arguments = filePath;

    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.RedirectStandardError = true;

    process.Start();

    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();

    logControl.WriteError(error);

    process.WaitForExit();
    return new PdfDocumentInfo(output, error);
  }
}
