using System;

namespace SplittingComponents;

public class StringExtensions
{
  public static string Truncate(string value, int maxLength, string suffix = "…")
  {
    if (value.Length <= maxLength)
    {
      return value;
    }
    return string.Concat(value.AsSpan(0, maxLength - suffix.Length), suffix);
  }

  public static string FilePath(string path, string fileName)
  {
    var fileInfo = fileName.Split('.');

    var file = Truncate(fileInfo[0], 100);
    var extension = fileInfo[1];

    return $"{path}{Path.DirectorySeparatorChar}{file}.{extension}";
  }

  public static string CsvEncode(string? input)
    => input?.Replace(",", ".") ?? string.Empty;
}

