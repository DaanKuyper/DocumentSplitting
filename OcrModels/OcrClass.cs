using Tesseract;

namespace OcrModels;

public class OcrClass : IDisposable
{
  public OcrClass()
  {
    Engine = new TesseractEngine(TessDataPath, "nld");
  }
    

  public (string, float) ProcessImage(byte[] imageBytes)
  {
    using var image = Pix.LoadFromMemory(imageBytes);
    using var page = Engine.Process(image);

    return (page.GetText(), page.GetMeanConfidence());
  }



  private readonly TesseractEngine Engine;

  private static string TessDataPath => @"./tessdata";


  public void Dispose() => Engine.Dispose();
}
