namespace WobModels;

public class WobFileList : List<WobFile>
{
  public void AddFile(WobFile? wobFile)
  {
    // TODO prettify...
    if (wobFile == null)
    {
      throw new Exception();
    }
    Add(wobFile);
  }
}

