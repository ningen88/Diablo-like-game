using System;
using System.Collections.Generic;
using UnityEngine;


public class StateMachine
{
    // private section
    private IState _currentState;
    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> _currentTransitions = new List<Transition>();
    private List<Transition> _anyTransitions = new List<Transition>();

    private List<Transition> _EmptyTransition = new List<Transition>(0);            // and empty list that we use when we want to set _currentTranstitions to empty


    // Tick() call the function Tick() for the current state and set the state if there's a transition
    public void Tick()
    {
        Transition transition = GetTransition();                                                // call GetTransition to get the next transition (the next transition is the
                                                                                                // first one with the condition = true)
        if(transition != null)                                                                  // if the transition is not null
        {
            SetState(transition._to);                                                           // set the state to the state that the transition leads to
        }

        _currentState?.Tick();                                                                  // call Tick() for the current state
    }

    public void FixTick()
    {

    }


    // SetState set  the curernt state and check if the current transition is null
    public void SetState(IState state)
    {
        if (state == _currentState) return;                                                      // if state is the current state we don't need to change it

        _currentState?.OnExit();                                                                 // call OnExit for the current state
        _currentState = state;                                                                   // set the current state to the new state

        _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);              // get the value of the dictionary entry using the current state type

        if(_currentTransitions == null)                                                          // if the entry is null
        {
            _currentTransitions = _EmptyTransition;                                              // set the current transitions list to an empty list
        }

        _currentState.OnEnter();                                                                 // call OnEnter for the current state
    }


    // add a new transition to the Dictionary
    public void AddTransition(IState from, IState to, Func<bool> condition)
    {
        if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)              // if we can't get an entry in the dictionary with the from type
        {
            transitions = new List<Transition>();                                                // create a new list of Transitions
            _transitions[from.GetType()] = transitions;                                          // insert the new Transitions list in the dictionary with the key from.GetType()
        }

        Transition newTransition = new Transition(condition, to);

        transitions.Add(newTransition);                                                          // add the transition in the transitions list of the dictionary
    }

    public void AddAnyTransition(IState state, Func<bool> condition)
    {
        _anyTransitions.Add(new Transition(condition, state));
    }


    // loop the current transitions list and return the first entry with a true condition
    public Transition GetTransition()
    {
                foreach (var transition in _anyTransitions)
                {
                    if (transition._condition())
                    {
                        return transition;
                    }
                }                


        foreach (var transition in _currentTransitions)
        {
            if (transition._condition())
            {
                return transition;              
            }
        }

        return null;
    }

    public void transitionState()
    {
        if(_currentTransitions.Count == 0)
        {
            Debug.Log("The list is empty");
            return;
        }

        foreach (var transition in _currentTransitions)
        {
            Debug.Log("Transizione: " + transition._to.GetType().ToString());
        }
    }

    public void dictionaryState()
    {
        if(_transitions.Count == 0)
        {
            Debug.Log("Dictionary is empty");
        }

        int i = 1;
        Debug.Log("Entry in dictionary:" + _transitions.Count);
        foreach(var entry in _transitions.Values)
        {
            Debug.Log("Entry: " + i);
            i++;
            foreach(var val in entry)
            {
                Debug.Log(val._to.GetType().ToString());
            }
        }
    }
}
