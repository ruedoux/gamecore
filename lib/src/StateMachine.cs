namespace Qwaitumin.GameCore;

public abstract class BaseState<EState> where EState : Enum
{
  public readonly EState StateKey;
  public Action<EState>? OnStateChangeRequested;

  protected BaseState(EState stateKey)
  {
    StateKey = stateKey;
  }

  public abstract void Enter();
  public abstract void Exit();

  protected void SwitchToState(EState stateKey)
  {
    if (OnStateChangeRequested != null)
      OnStateChangeRequested(stateKey);
    else
      throw new InvalidOperationException("State was not registered.");
  }
}

public abstract class StateMachine<EState> where EState : Enum
{
  protected Dictionary<EState, BaseState<EState>> stateMap = new();
  protected BaseState<EState>? CurrentState = null;

  protected StateMachine(BaseState<EState>[] states)
  {
    foreach (var state in states)
    {
      state.OnStateChangeRequested = SwitchState;
      stateMap[state.StateKey] = state;
    }
  }

  public void SwitchState(EState stateKey)
  {
    if (!stateMap.TryGetValue(stateKey, out var newState))
      throw new ArgumentException($"State '{stateKey}' not found in state map.");

    CurrentState?.Exit();
    CurrentState = stateMap[stateKey];
    CurrentState.Enter();
  }
}