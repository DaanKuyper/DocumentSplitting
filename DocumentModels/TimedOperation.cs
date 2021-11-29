using System.Diagnostics;

namespace SplittingComponents;

public class TimedOperation
{
  public TimedOperation()
  {
    Iteration = 0;

    Stopwatch = new Stopwatch();
    Stopwatch.Start();
  }

  public void IterationPassed() => Iteration++;

  public TimeSpan CompleteOperation()
  {
    Stopwatch.Stop();
    return Stopwatch.Elapsed;
  }


  public readonly Stopwatch Stopwatch;

  public int Iteration;
}

