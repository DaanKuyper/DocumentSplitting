namespace SplittingComponents;

public class ConfigException : Exception
{
  public ConfigException(string configFile) : base($"Configuration problem encountered : `{configFile}`")
  {
  }
}
