using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerState
{
    #region ״̬�����
    protected NewPlayerStateMachine stateMachine;
    protected NewPlayerController player;
    #endregion
    #region ���

    #endregion
    #region ����
    protected string animBoolName;
    protected bool stateEnd;
    #endregion
    public NewPlayerState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        if (player.isNewAC)
        {

        }
        else
        {
            player.thisAC.TBool(animBoolName);

        }
        stateEnd = false;
    }

    public virtual void Update()
    {

    }
    public virtual void FixedUpdate()
    {

    }

    public virtual void Exit()
    {
        if (player.isNewAC)
        {

        }
        else
        {
            player.thisAC.FBool(animBoolName);
        }
    }
    public void CurrentStateEnd()//�����жϿ��ж�״̬����̻򹥻�
    {
        stateEnd = true;
    }
    protected virtual void CurrentStateCandoChange()
    {

    }
    protected virtual void CurrentStateCandoUpdate()
    {

    }
}
