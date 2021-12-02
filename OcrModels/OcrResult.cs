namespace OcrModels;

public class OcrResult
{
  public OcrResult(Page page)
  {
    Text = page.GetText();
    Confidence = page.GetMeanConfidence();
  }

  public string Text { get; set; }

  public float Confidence { get; set; }
}
