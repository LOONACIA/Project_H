using System.Collections.Generic;

public class StateMachine<T> 
    where T : class
{
	private readonly List<StateBase<T>> m_globalStates = new();
    
    // Define a private variable called _states of type Dictionary<State, StateBase>
    private Dictionary<State, StateBase<T>> m_states = new();

    // Define a private variable called _currentState of type StateBase
    private StateBase<T> m_currentState;

    private StateBase<T> m_previousState;

    // Define a property called CurrentState of type StateBase
    public StateBase<T> CurrentState => m_currentState;

    // Define a method called AddState that takes a StateBase as parameters
    public void AddState(StateBase<T> state)
    {
        m_states.Add(state.Type, state);
    }

    public void AddGlobalState(StateBase<T> state)
    {
        m_globalStates.Add(state);
    }

    // Define a method called ChangeState that takes a State as a parameter
    public void ChangeState(State state)
    {
        // If _currentState is not null
        // Call Exit on _currentState
        m_currentState?.Exit();
        
        // Set _previousState to _currentState
        m_previousState = m_currentState;

        // Find Key in dictionary, and Set _currentState to value. if not found, set to Idle
        m_currentState = m_states.TryGetValue(state, out var value) ? value : m_states[State.Idle];

        // Call Enter on _currentState
        m_currentState.Enter();
    }

    // Define a method called Execute that takes no parameters
    public void Execute()
    {
        foreach (var state in m_globalStates)
        {
            state.Execute();
        }
        
        // If _currentState is not null
        // Call Update on _currentState
        m_currentState?.Execute();
    }

    // Define a method called FixedExecute that takes no parameters
    public void FixedExecute()
    {
        foreach (var state in m_globalStates)
        {
            state.FixedExecute();
        }
        
        // If _currentState is not null
        // Call FixedUpdate on _currentState
        m_currentState?.FixedExecute();
    }
    
    public void RevertToPreviousState()
    {
        ChangeState(m_previousState.Type);
    }
}