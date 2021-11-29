namespace SplittingComponents;

public class LogController
{
  public LogController(string logFile)
  {
    LogFile = logFile;

    Output(Environment.NewLine);
    Output("---------------------------------");
    Write("New Log Started");
  }


  public void StartOperation(string operationName)
  {
    TimedOperations[operationName] = new TimedOperation();
  }

  public void IterationPassed(string operationName, string? identifier = null)
  {
    TimedOperations[operationName].IterationPassed();

    Write($"{TimedOperations[operationName].Iteration} {identifier}" +
      $" passed for operation `{operationName}`");
  }

  public void FinishOperation(string operationName)
  {
    var operationTime = TimedOperations[operationName].CompleteOperation();
    
    Write($"Operation `{operationName}` completed, took {operationTime}");

    TimedOperations.Remove(operationName);
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

