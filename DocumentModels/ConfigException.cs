namespace SplittingComponents;

public class ConfigException : Exception
{
  public ConfigException(string message)
    : base($"Configuration problem encountered : `{message}`")
  {
  }
}
