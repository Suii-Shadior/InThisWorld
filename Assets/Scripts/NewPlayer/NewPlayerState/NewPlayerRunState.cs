using PlayerInterfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class NewPlayerRunState : NewPlayerState, IMove_horizontally
{
    public NewPlayerRunState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        /* 
         * Work1.继承状态机进入逻辑
         * Work2.进入HorizontalMove逻辑
         * Work3.进入帧Can判断
         * 
         */
        base.Enter();
        MoveEnter();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        //KeepInertiaCount();
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
         * Work1.开跑时仍然可以左右翻面
         * Work2.开跑时仍然可以左右位移
         * Work3.开跑时不能上下位移
         * Work4.开跑时理应还在地面，进行跳跃刷新，然后Can跳跃一定会判定成true
         */
        base.CurrentStateCandoChange();
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();
        player.WhetherCanInteract();
    }

    protected override void CurrentStateCandoUpdate()
    {
        /* 本方法用于持续的进行Can判断，适用于普通帧
         * 
         * Work1.跑步时时刻刷新延后跳的口水鸡哦，检测Can跳跃
         * Work2.下落时时刻检测Can道具使用
         * Work3.下落时时刻检测Can交互
         */
        base.CurrentStateCandoUpdate();
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();
        player.WhetherCanInteract();

    }
    private void MoveEnter()
    {

        /* 
         * Step1.启用碰撞体
         * Step2.player运动参数设置
         * Step3.进入门槛速度
         */
        player.thisBoxCol.enabled = true;

        player.horizontalMoveSpeedAccleration = player.playerConfig.idle_MoveAccleration;
        player.horizontalMoveSpeedMax = player.playerConfig.idle_MoveSpeedMax;
        player.horizontalmoveThresholdSpeed = player.playerConfig.idle_MoveThresholdSpeed;
        HorizontalMove();
    }



    private void WhetherExit()
    {

        /* 
         * Work1.horizontalMove=>fall
         * Work2.horizontalMove=>idle
         * 
         */
        if (!player.thisPR.IsOnFloored())
        {
            player.thisPR.LeaveFloor();
            player.ChangeToFallState();
            return;
        }
        else if (player.horizontalInputVec == 0)
        {
            //Debug.Log("停下");
            player.ChangeToIdleState();
            return;
        }
    }

    #region 接口实现
    public void HorizontalMove()
    {
        /* 本接口方法用于通过方向键实现不同状态下的水平位移
         * 
         * 
         * Step1.判断是否在gameplay中.若是，进入Step2;若否，无内容
         * Step2.判断是否isUncontrol。若是，进入Step3-a；若否，进入Step3-b；
         * Step3-a.
         * Step3-b.根据是否有输入、输入方向是否与当前面朝方向相同、当前的水平位移速度进行不同的逻辑判断。
         *      若面朝方向和输入方向一致——当速度大于等于最高速度，则在面朝方向上维持最高速度；
         *                                  当速度小于最高速度且大于门槛速度，则在面朝方向进行加速；
         *                                  当速度小于门槛速度，则在面朝方向以门槛速度移动。
         *      若面朝方向和输入方向不一致——当速度大于门槛速度，则在面朝方向上进行减速；
         *                                    当毒素小于等于门槛速度，则以输入方向以门槛速度移动//TODO：如果加入了转身动作，其实应该在原地停留几秒等待镜头转向
         *      若无输入——当速度大于门槛速度，则在面朝方向上进行减速；
         *                  当毒素小于等于门槛速度，则面朝速度置0
         * 
         */
        if (player.GetIsGamePlay())
        {
            if (player.GetIsUncontrol())
            {
                
            }
            else
            {
                if (player.horizontalInputVec != 0)
                {
                    if (player.faceDir != player.horizontalInputVec)
                    {
                        player.ClearXVelocity();
                        player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalmoveThresholdSpeed, 0f);
                    }
                    else
                    {
                        if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalMoveSpeedMax)
                        {
                            if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed)
                            {
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalmoveThresholdSpeed, 0f);
                            }
                            else
                            {
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedAccleration, 0f);
                            }
                        }
                        else
                        {
 
                            player.ClearXVelocity();
                            player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedMax, 0f);
                        }
                    }
                }
                else
                {
                    if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed || player.thisPR.IsOnWall())
                    {
                        player.ClearXVelocity();
                    }
                    else
                    {
                        player.thisRB.velocity += new Vector2(-player.faceDir * player.horizontalMoveSpeedAccleration, 0f);
                    }
                }
            }
        }
    }
    #endregion
}
