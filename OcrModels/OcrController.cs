namespace OcrModels;

public class OcrController : IDisposable
{
  public OcrController()
  {
    Engine = new TesseractEngine(TessDataPath, "nld");
  }
    

  public OcrResult ProcessImage(byte[] imageBytes)
  {
    using var image = Pix.LoadFromMemory(imageBytes);
    using var page = Engine.Process(image);

    return new OcrResult(page);
  }



  private readonly TesseractEngine Engine;

  private static string TessDataPath => @"./tessdata";


  public void Dispose() => Engine.Dispose();
}
