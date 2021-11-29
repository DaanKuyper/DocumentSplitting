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


  public string BaseUrl { get; set; } = string.Empty;

  public string ApiUrl { get; set; } = string.Empty;


  public string LocalMetaStoragePath { get; set; } = string.Empty;

  public string LocalPdfStoragePath { get; set; } = string.Empty;

  public string LocalReportFile { get; set; } = string.Empty;

  public string OutputLogFile { get; set; } = string.Empty;


  public bool UseLocalWobMetaData { get; set; } = false;
}
