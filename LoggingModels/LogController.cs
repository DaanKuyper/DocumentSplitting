namespace LoggingModels;

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
    Write($"Started new Operation: `{operationName}`");
  }

  public void IterationPassed(string operationName, string? identifier = null)
  {
    var iterationTime = TimedOperations[operationName].CompleteIteration();

    Write($"{TimedOperations[operationName].Iteration} {identifier}" +
      $" passed for operation `{operationName}`, took {iterationTime}");
  }

  public void FinishOperation(string operationName)
  {
    var operationTime = TimedOperations[operationName].CompleteOperation();

    Write($"Operation `{operationName}` completed, took {operationTime}");

    TimedOperations.Remove(operationName);
  }


  public void Write(string function, string message)
    => Write($"{FunctionPrefix(function)}{message}");


  public void Write(string message)
  {
    if (!string.IsNullOrWhiteSpace(message))
    {
      Output(message);
    }
  }

  public void WriteError(string message, string? identifier = null)
  {
    if (!string.IsNullOrWhiteSpace(message))
    {
      Output(!string.IsNullOrWhiteSpace(identifier) ?
        $"Exception encountered for : `{identifier}`" : 
        $"Exception encountered:");

      Output($" -> Exception message: {message}");
    }
  }

  private void Output(string logOutput)
  {
    File.AppendAllText(LogFile, LogFileOutput(logOutput));
    Console.WriteLine(logOutput);
  }


  private static string FunctionPrefix(string name)
    => $"{(!string.IsNullOrWhiteSpace(name) ? name : "Unknown")} : ";

  private static string DatePrefix => $"[{DateTime.Now}] - ";

  private static string LogFileOutput(string logOutput)
    => $"{DatePrefix}{logOutput}{Environment.NewLine}";



  private readonly Dictionary<string, TimedOperation> TimedOperations = new();

  private readonly string LogFile;
}

