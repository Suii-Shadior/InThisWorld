using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallFallState : PlayerState
{
    public PlayerWallFallState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.fallSpeedMax = player.wallFallSpeedMax;
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        CurrentStateCandoUpdate();
        player.WhetherHoldOrWallFall();
        player.Fall();
    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.canJumpCounter = 0;
        player.canWallJump = true;
        player.canHold = true;
        player.canWallFall = true;
        player.canAttack = false;
    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanJump();
        player.WhetherCanHold();
        player.WhetherCanWallFall();
        player.WhetherCanWallVeritalForward();
        player.WhetherCanDash();
    }
}
