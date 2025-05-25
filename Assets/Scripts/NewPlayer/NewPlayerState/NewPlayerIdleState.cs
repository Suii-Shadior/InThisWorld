using PlayerInterfaces;
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
        /* 
         * Work1.继承状态机进入逻辑
         * Work2.idle进入逻辑
         * Work3.进入帧Can判断
         * 
         */
        base.Enter();
        IdleEnter();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {   
        /* 
         * 
         * Work1.普通帧Can判断
         * Work2.Fall逻辑 TODO：其实涉及了物理判断的，应该放在FixedUpdate
         * Work3.退出逻辑检测
         * 
         */
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
        /* 本方法用于进行一次状态下Can判断进入变化，适用于进入帧
         * 
         * Work1.进入Idle时，可以转向
         * Work2.进入Idle时，可以水平移动，不能垂直移动
         * Work3.进入Idle时，刷新延迟跳计时，判断Can跳跃
         * Work4.进入Idle时，进行Can道具使用判断
         * Work5.进入Idle时，进行Can交互判断
         * 
         */
        base.CurrentStateCandoChange();
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();
    }

    protected override void CurrentStateCandoUpdate()
    {
        /* 本方法用于持续的进行Can判断，适用于普通帧
         * 
         * Work1.Idle阶段时，时刻判断刷新跳跃，并进行跳跃判定
         * Work2.Idle阶段时，默认还是在地面上，所以刷新跳跃，并进行跳跃判定
         * Work3.Idle阶段时，进行Can道具交互判断
         * Work4.Idle阶段时，进行Can交互判断
         * 
         * 
         */
        base.CurrentStateCandoUpdate();
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();
    }



    private void IdleEnter()
    {
        /* 
         * Step1.清空Player的速度,同时根据落地对象清空进行额外的位置补正
         * Steo2.恢复碰撞体
         * Step3.player相关参数变化
         */
        player.ClearYVelocity();

        if(player.thisPR.IsOnFloored()&&!player.thisPR.WasOnFloored())
        //if (player.isStandOnPlatform() && !player.thisPR.wasFloored) 
        {
            if (player.isStandOnPlatform())
            //Debug.Log("落上平台");
            player.transform.position -=new Vector3(0, player.thisPR.RayHit().distance,0);
        }
        player.thisBoxCol.enabled = true;


        player.horizontalMoveSpeedAccleration = player.playerConfig.idle_MoveAccleration;
        player.horizontalMoveSpeedMax = player.playerConfig.idle_MoveSpeedMax;
        player.horizontalmoveThresholdSpeed = player.playerConfig.idle_MoveThresholdSpeed;
    }

    private void WhetherExit()
    {
        /* 
         * Work1.idle=>fall
         * Work2.idle=>horizontalMove
         * 
         * 
         */
        if (!player.thisPR.IsOnFloored())
        {
            player.thisPR.LeaveFloor();
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
        if (player.GetIsUncontrol())//受限移动时候的移动
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
