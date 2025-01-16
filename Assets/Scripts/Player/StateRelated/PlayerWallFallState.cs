using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWallFallState : PlayerState
{
    public PlayerWallFallState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.verticalFallSpeedMax = player.wallFallSpeedMax;
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
        player.Fall();
        WhetherExit();
        player.WhetherHoldOrWallFall();
    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
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

        if (player.thisPR.IsOnGround())
        {
            player.needTurnAround = true;
            player.thisAC.FlipX();
            stateMachine.ChangeState(player.idleState);
        }
        else
        {
            
            player.WhetherHoldOrWallFall();
        }
    }
}
