using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHoldState : PlayerState

{
    public PlayerHoldState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.Hold();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
        player.EndHold();
    }

    public override void Update()
    {
        base.Update();
        CurrentStateCandoUpdate();
        player.Fall();
        WhetherExit();

    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canHorizontalMove = false;
        player.canVerticalMove = true;
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanDash();
        player.WhetherCanHold();
        //刚进入贴墙落状态，肯定可以继续该状态
        player.canAttack = false;
    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanHold();
        player.WhetherCanWallFall();
        player.WhetherCanDash();
        //player.WhetherCanWallVeritalForward();
    }
    private void WhetherExit()
    {
        player.WhetherHoldOrWallFall();
    }
}
