namespace Qwaitumin.GameCore;

public class ObserverNotifier<T>
{
  private readonly List<Action<T>> _observers = new();

  public ObserverNotifier(IEnumerable<Action<T>> observers)
    => AddObservers(observers);

  public void AddObservers(IEnumerable<Action<T>> observers)
    => _observers.AddRange(observers);

  public void RemoveObserver(Action<T> observer)
    => _observers.Remove(observer);

  public Action<T>[] GetObservers()
    => _observers.ToArray();

  public void NotifyObservers(T arg)
  {
    foreach (var observer in _observers) observer(arg);
  }
}


public class SignalEmitter
{
  private readonly List<Action> _signals = new();

  public SignalEmitter(IEnumerable<Action> signals)
    => AddSignals(signals);

  public void AddSignals(IEnumerable<Action> signals)
    => _signals.AddRange(signals);

  public void RemoveSignal(Action signal)
    => _signals.Remove(signal);

  public Action[] GetSignals()
    => _signals.ToArray();

  public void EmitSignal()
  {
    foreach (var signal in _signals)
      signal();
  }
}