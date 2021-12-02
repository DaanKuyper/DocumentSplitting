namespace PdfModels;

public class XPdfController
{
  // TODO : support Linux & MacOS for xpdf commands...

  public static void PdfToHtml(string filePath, string folderPath,
    string folderName, bool overwriteExisting, LogController logControl)
  {
    var path = $"{folderPath}{Path.DirectorySeparatorChar}{folderName}";

    if (Directory.Exists(path))
    {
      if (!overwriteExisting)
      {
        logControl.Write($"Path existed `{path}` - skipping...");
        return;
      }

      logControl.Write($"Path existed `{path}` - overwriting...");
      Directory.Delete(path, true);
    }

    var process = new Process();
    process.StartInfo.FileName = $"xpdf{Path.DirectorySeparatorChar}pdftohtml";
    process.StartInfo.Arguments = $"{filePath} {path}";

    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.RedirectStandardError = true;

    process.Start();

    logControl.Write(process.StandardOutput.ReadToEnd());
    logControl.WriteError(process.StandardError.ReadToEnd(), folderName);

    process.WaitForExit();
  }


  public static void PdfToText(string filePath, string resultPath,
    LogController logControl)
  {
    var process = new Process();
    process.StartInfo.FileName = $"xpdf{Path.DirectorySeparatorChar}pdftotext";
    process.StartInfo.Arguments = $"{filePath} {resultPath}";

    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.RedirectStandardError = true;

    process.Start();

    logControl.Write(process.StandardOutput.ReadToEnd());
    logControl.WriteError(process.StandardError.ReadToEnd(), filePath);

    process.WaitForExit();
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
