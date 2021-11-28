namespace SplittingComponents;

public class DataException : Exception
{
  public DataException(string message)
        : base($"Data problem encountered : `{message}`")
  {
  }
}

