using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.WallJump();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
        player.WallJumpdEnd();
    }

    public override void Update()
    {
        base.Update();
        CurrentStateCandoUpdate();
        player.Move();
        if (player.thisRB.velocity.y < -player.peakSpeed)
        {
            stateMachine.ChangeState(player.airState);
            return;
        }
    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canHorizontalMove = false;
        player.canVerticalMove = false;
        player.canJumpCounter = 0;
        player.canWallJump = false;
        player.canHold = false;
        player.canWallFall = true;
        player.canAttack = false;


    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanHold();
        player.WhetherCanWallFall();
        player.WhetherCanDash();
        if (player.wallJumpPostCounter > 0) player.wallJumpPostCounter -= Time.deltaTime;
        else
        {
            player.WallJumpdEnd();
        }
    }
}
