using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine

{
    public PlayerState currentState { get; private set; }
    //������Ϸ�ĵ�һ��״̬
    public void Initialize(PlayerState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }
    //�ı�״̬
    public void ChangeState(PlayerState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}