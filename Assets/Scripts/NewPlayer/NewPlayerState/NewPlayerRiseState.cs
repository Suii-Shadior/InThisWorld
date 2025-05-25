using PlayerInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerRiseState : NewPlayerState, IMove_horizontally,IJump
{
    public NewPlayerRiseState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        /* 
         * Work1.继承状态机进入逻辑
         * Work2.进入Rise逻辑
         * Work3.跳跃动作
         * Work3.进入帧Can判断
         * 
         */
        base.Enter();
        RiseEnter();
        Jump();
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
        WhetherExit();

    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        HorizontalMove();//在跳起过程中X轴移动速度仍然保持相同的加速度
    }
    protected override void CurrentStateCandoChange()
    {
        /* 本方法用于进行一次状态下Can判断进入变化，适用于进入帧
         * 
         * Work1.进入Rise时，可以转向
         * Work2.进入Rise时，可以水平移动，不能垂直移动
         * Work3.进入Rise时，判断Can跳跃,主要是但是有可能吃到一些道具，使得实现可以无限跳跃，所以依旧要判断是否能跳跃
         * Work4.进入Rise时，进行Can道具使用判断
         * Work5.进入Rise时，进行Can交互判断
         * 
         */
        base.CurrentStateCandoChange();
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();

    }

    private void RiseEnter()
    {
        /* 
         * Work1.恢复碰撞体
         * Work3.player相关参数变化
         */
        player.thisBoxCol.enabled = false;

        player.horizontalMoveSpeedAccleration = player.playerConfig.air_MoveAccleration;
        player.horizontalmoveThresholdSpeed = player.playerConfig.air_MoveThresholdSpeed;
        player.horizontalMoveSpeedMax = player.playerConfig.air_MoveSpeedMax;
        player.verticalFallSpeedMax = player.playerConfig.air_FallSpeedMax;
    }

    private void WhetherExit()//
    {
        /* 
         * Work1.rise=>fall
         * Work2.rise=>apex/TODO:理论上上升途中不会被
         */
        if (player.thisPR.IsHead())
        {
            player.ClearYVelocity();
            player.ChangeToFallState();
        }
        else if (player.thisRB.velocity.y < 0)
        {

            player.ChangeToFallState();
        } 
        //if (player.thisRB.velocity.y < -player.peakSpeed) //要添加peak状态时启用
        //{
        //    player.ChangeToFallState();  
        //}
    }

    protected override void CurrentStateCandoUpdate()
    {
        /* 本方法用于持续的进行Can判断，适用于普通帧
         * 
         * Work1,Rise时，检测Can跳跃,因为可能吃到一些道具，使得实现可以无限跳跃，所以依旧要判断是否能跳跃
         * Work2.Rise时,下落时时刻检测Can道具使用
         * Work3.Rise时,下落时时刻检测Can交互
         */
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();

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
         *                                    当毒素小于等于门槛速度，则以输入方向以门槛速度移动
         *      若无输入——当速度大于门槛速度，则在面朝方向上进行减速；
         *                  当毒素小于等于门槛速度，则面朝速度置0
         * 
         */
        if (player.GetIsGamePlay())
        {
            if (player.GetIsUncontrol())
            {
                //
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
    public void Jump()
    {

        /* 本接口方法用于实现不同状态下的跳跃
         * 
         * Step1.将延迟跳计时置0
         * Step2.清空纵轴速度
         * Step3.在垂直方向施加力用于摸你跳跃//TODO：更改物理实现方式，方式要变
         */
        player.CoyoteCounterZero();
        player.ClearYVelocity();
        player.thisRB.AddForce(Vector2.up * player.playerConfig.normal_JumpForce, ForceMode2D.Impulse);
        //Debug.Log(thisRB.velocity.y);
    }
    #endregion
}
