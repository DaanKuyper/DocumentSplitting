namespace SplittingComponents;

public class LogController
{
  public LogController(string logFile)
  {
    LogFile = logFile;
  }


  public void StartOperation(string operationName)
  {
    TimedOperations[operationName] = new TimedOperation();
  }

  public void IterationPassed(string operationName, string? message = null)
  {
    TimedOperations[operationName].IterationPassed();

    var output = $"{TimedOperations[operationName].Iteration} passed for operation `{operationName}`";
    if (!string.IsNullOrWhiteSpace(message))
    {
      output += $": {message}";
    }
    Write(output);
  }

  public void FinishOperation(string operationName)
  {
    var operationTime = DateTime.Now - TimedOperations[operationName].StartTime;
    
    Write($"Operation `{operationName}` completed, took {operationTime}");

    TimedOperations.Remove(operationName);
  }


  public void Write(Exception exception, bool isBreaking = true)
  {
    Write(exception.Source ?? string.Empty, exception.Message);
    if (isBreaking)
    {
      throw exception;
    }
  }


  public void Write(string function, string message)
    => Write($"{FunctionPrefix(function)}{message}");


  public void Write(string message) => Output($"{Prefix}{message}");


  private void Output(string logOutput)
  {
    Console.WriteLine(logOutput);
    File.AppendAllText(LogFile, logOutput + Environment.NewLine);
  }


  private static string FunctionPrefix(string name)
    => $"{(!string.IsNullOrWhiteSpace(name) ? name : "Unknown")} : ";

  private static string Prefix => $"[{DateTime.Now}] - ";


  private readonly Dictionary<string, TimedOperation> TimedOperations = new();

  private readonly string LogFile;
}

