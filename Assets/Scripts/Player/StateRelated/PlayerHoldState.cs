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
        player.Fall();
        if (player.canVerticalMove && player.verticalInputVec != 0)
        {
            stateMachine.ChangeState(player.wallClimbState);
            return;
        }
        CurrentStateCandoUpdate();
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
        player.WhetherCanJump();
        player.WhetherCanHold();
        player.WhetherCanWallVeritalForward();
        player.WhetherCanWallFall();
        player.WhetherCanDash();
    }
}
