namespace PdfModels;

public class PdfPageClass
{
  public PdfPageClass(PdfPage page)
  {
    var cSequence = ContentReader.ReadContent(page);

    var result = new StringBuilder();
    foreach (var cObject in cSequence)
    {
      ExtractText(cObject, result);
    }
    Content = result.ToString();
  }


  void ExtractText(CObject cObject, StringBuilder result)
  {
    if (cObject is COperator)
    {
      var cOperator = cObject as COperator;
      if (cOperator != null)
      {
        if (cOperator.OpCode.Name == OpCodeName.Tf.ToString())
        {
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
    }
    else if (cObject is CSequence)
    {
      var cSequence = cObject as CSequence;
      if (cSequence != null)
      {
        foreach (var element in cSequence)
        {
          ExtractText(element, result);
        }
      }
    }
    else if (cObject is CString)
    {
      var cString = cObject as CString;
      if (cString != null)
      {
        WordCount++;
        result.Append(cString.Value);
        result.Append(' ');
      }
    }
  }

  public string Content;

  public int WordCount;
}
