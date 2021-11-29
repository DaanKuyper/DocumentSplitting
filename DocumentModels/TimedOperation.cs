namespace SplittingComponents;

public class TimedOperation
{
  public TimedOperation()
  {
    StartTime = DateTime.Now;
    Iteration = 0;
  }

  public void IterationPassed() => Iteration++;

  public readonly DateTime StartTime;

  public int Iteration;
}

