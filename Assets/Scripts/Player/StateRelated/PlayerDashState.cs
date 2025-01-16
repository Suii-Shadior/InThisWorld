using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.Dash();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
        player.DashEnd();
    }

    public override void Update()
    {
        base.Update();
        CurrentStateCandoUpdate();
        player.DashKeep();
    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canHorizontalMove = false;
        player.canVerticalMove = false;
        player.canJumpCounter = 0;
        player.canHold = false;
        player.canWallFall = true;
        player.canAttack = false;
        player.canDash = false;
    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanHold();
        player.WhetherCanWallFall();
        player.WhetherCanDash();
    }
}
