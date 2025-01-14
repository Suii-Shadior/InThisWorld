using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState currentState { get; private set; }
    // Start is called before the first frame update
    public void Initialize(PlayerState _playerState)
    {

        currentState = _playerState;
        currentState.Enter();

    }

    public void ChangeState(PlayerState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
