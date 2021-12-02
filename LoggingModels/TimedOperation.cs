namespace LoggingModels;

public class TimedOperation
{
  public TimedOperation()
  {
    Iteration = 0;

    StartTime = new Stopwatch();
    OperationTime = new Stopwatch();

    StartTime.Start();
    OperationTime.Start();
  }

  public TimeSpan CompleteIteration()
  {
    OperationTime.Stop();
    var iterationTime = OperationTime.Elapsed;
    OperationTime.Restart();

    Iteration++;
    return iterationTime;
  }

  public TimeSpan CompleteOperation()
  {
    StartTime.Stop();
    return StartTime.Elapsed;
  }


  public readonly Stopwatch StartTime;

  public readonly Stopwatch OperationTime;

  public int Iteration;
}

