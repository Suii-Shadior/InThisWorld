using PlayerInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewPlayerFallState : NewPlayerState, IFall_vertically, IMove_horizontally
{
    public NewPlayerFallState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    public override void Enter()
    {
        /* 
         * Work1.继承状态机进入逻辑
         * Work2.Fall进入逻辑
         * Work3.进入帧Can判断
         * 
         */
        base.Enter();
        FallEnter();
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
        base.Update();//空内容
        //KeepInertiaCount();//先注释掉，可以改
        CurrentStateCandoUpdate();
        Fall();
        WhetherExit();

    }
    public override void FixedUpdate()
    {
        /*
         * 水平位移逻辑
         */
        base.FixedUpdate();
        HorizontalMove();

    }

    protected override void CurrentStateCandoChange()
    {
        /* 本方法用于进行一次状态下Can判断进入变化，适用于进入帧
         * 
         * Work1.下落时仍然可以左右翻面
         * Work2.下落时仍然可以左右位移
         * Work3.下落时不能上下位移
         * Work4.下落时进行起跳判断，操作逻辑上来看不可能马上又能跳跃的，因为在进入帧已经将延后跳时间置0，所以通过此方法进行判断比直接置否要合理一点。主要是但是有可能吃到一些道具，使得实现可以无限跳跃，所以依旧要判断是否能跳跃
         */
        base.CurrentStateCandoChange();//空内容
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;

        player.WhetherCanJumpOrWallJump();
        

    }

    protected override void CurrentStateCandoUpdate()
    {
        /* 本方法用于持续的进行Can判断，适用于普通帧
         * 
         * Work1.下落时时刻检测Can跳跃
         * Work2.下落时时刻检测Can道具使用
         * Work3.下落时时刻检测Can交互
         */
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();
        player.WhetherCanInteract();

    }

    private void FallEnter()
    {

        player.thisBoxCol.enabled = true;
        player.horizontalMoveSpeedAccleration = player.playerConfig.air_MoveAccleration;
        player.horizontalmoveThresholdSpeed = player.playerConfig.air_MoveThresholdSpeed;
        player.horizontalMoveSpeedMax = player.playerConfig.air_MoveSpeedMax;
        player.verticalFallSpeedMax = player.playerConfig.air_FallSpeedMax;

    }
    public void WhetherExit()
    {
        /* 
         * fall => idle
         */
        if (player.thisPR.IsOnFloored())
        {

            player.ChangeToIdleState();
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
    }
    public void Fall()
    {
        /* 本接口方法用于实现不同状态下的自由下落
         * 
         * 判断当前速度是否小于等于下落最大速度。若是，朝下落方向以最大速度移动，若否，无内容（通过重力向下落方向进行加速）
         * TODO：输入重力方向应该怎加速下落速度及最大下落速度。
         */

        if (player.thisRB.velocity.y < -player.verticalFallSpeedMax)
        {
            player.ClearYVelocity();
            player.thisRB.velocity += new Vector2(0, -player.verticalFallSpeedMax);
            //Debug.Log("最大速度:"+player.thisRB.velocity.y);
        }
        else
        {

        }
    }

    #endregion

}
