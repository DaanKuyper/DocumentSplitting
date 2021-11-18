namespace SplittingComponents;

public class Configuration
{
  public static Configuration Load(string configFile)
  {
    Configuration config;

    if (File.Exists(configFile))
    {
      var jsonString = File.ReadAllText(configFile);
      config = JsonSerializer.Deserialize<Configuration>(jsonString) ??
        throw new ConfigException(configFile);
    }
    else
    {
      config = new Configuration();
      File.WriteAllText(configFile, JsonSerializer.Serialize(config));
    }
    return config;
  }

  public string? JsonPath { get; set; }
}
