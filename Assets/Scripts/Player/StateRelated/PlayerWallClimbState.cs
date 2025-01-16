using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallClimbState : PlayerState
{
    public PlayerWallClimbState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.ClimbPrepare();
    }

    public override void Exit()
    {
        base.Exit();
        player.ClimbEnd();
    }

    public override void Update()
    {
        base.Update();
        CurrentStateCandoUpdate();
        player.WhetherHoldOrWallFall();
        player.Climb();
    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canHorizontalMove = false;
        player.canVerticalMove = true;
        player.canJumpCounter = 0;
        player.canWallJump = true;
        player.canWallFall = true;
        player.canAttack = false;
        player.canHold = true;
    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanHold();
        player.WhetherCanWallFall();
        player.WhetherCanDash();
        //player.WhetherCanWallVeritalForward();
    }
}
