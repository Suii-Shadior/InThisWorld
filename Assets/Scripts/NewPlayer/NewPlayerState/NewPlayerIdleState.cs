using MoveInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerIdleState : NewPlayerState, IMove_horizontally
{
    public NewPlayerIdleState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        CurrentStateCandoChange();
        IdleEnter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        CurrentStateCandoUpdate();
        WhetherExit();


    }


    public override void FixedUpdate()
    {
        base.FixedUpdate();
        HorizontalMove();

    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanAttack();

    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanAttack();
    }

    private void IdleEnter()
    {

        player.ClearYVelocity();
        if (player.isStandOnPlatform() && !player.thisPR.wasFloored) 
        {
            //Debug.Log("落上平台");
            player.transform.position -=new Vector3(0, player.thisPR.RayHit().distance,0);
        }
        player.thisBoxCol.enabled = true;
        player.horizontalMoveSpeedAccleration = player.normalmoveAccleration; 
        player.horizontalMoveSpeedMax = player.normalmoveSpeedMax;
        player.horizontalmoveThresholdSpeed = player.normalmoveThresholdSpeed;
    }

    private void WhetherExit()
    {
        if (!player.thisPR.IsOnFloored())
        {
            player.thisPR.LeaveGround();
            player.ChangeToFallState();
            return;
        }
        else
        if (player.horizontalInputVec != 0)
        {
            
            player.ChangeToHorizontalMoving();
            return;
        }
    }



    public void HorizontalMove()
    {
        if (player.isUncontrol)//受限移动时候的移动
        {
            //
        }
        else
        {
            if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed||player.thisPR.IsOnWall())
            {
                //当前速度小于等于启动速度，则停止
                player.ClearXVelocity();
            }
            else
            {
                //当前速度大于启动速度，则减速
                player.thisRB.velocity += new Vector2(-player.faceDir * player.horizontalMoveSpeedAccleration, 0f);
            }

        }
    }
}
