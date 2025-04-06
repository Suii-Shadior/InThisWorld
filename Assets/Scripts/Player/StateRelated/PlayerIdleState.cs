using System.Diagnostics;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    // Start is called before the first frame update
    public PlayerIdleState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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
        player.WhetherCanDash();
        player.WhetherCanHold();
        player.canWallFall = false;
        player.canAttack = true;

    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanHold(); 
        player.WhetherCanWallFall();
        player.WhetherCanDash();
    }

    private void Idle()
    {
        if (!player.thisPR.IsOnFloored())
        {
            player.ChangeToAirState();
        }
        else 
        if (player.horizontalInputVec!=0)
        {
            player.ChangeToHorizontalMoving();
        }
    }
}
