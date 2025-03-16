using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerStateMachine
{
    public NewPlayerState currentState { get; private set; }
    // Start is called before the first frame update
    public void Initialize(NewPlayerState _playerState)
    {

        currentState = _playerState;
        currentState.Enter();

    }

    public void ChangeState(NewPlayerState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
