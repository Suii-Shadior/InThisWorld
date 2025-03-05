using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerIdleState : NewPlayerState
{
    public NewPlayerIdleState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
        player.canCooldown = true;
    }

    public override void Update()
    {
        base.Update();
        Idle();


    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.canAttack = true;

    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
    }

    private void Idle()
    {
        if (!player.thisPR.IsOnGround())
        {
            player.ChangeToFallState();
        }
        else
        if (player.horizontalInputVec != 0)
        {
            player.ChangeToHorizontalMoving();
        }
    }
}
