using MoveInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerRiseState : NewPlayerState, IMove_horizontally
{
    public NewPlayerRiseState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Jump();
        RiseEnter();
        CurrentStateCandoChange();
    }
    public void Jump()
    {
        //Time.timeScale = 0.1f;
        player.releaseDuringRising = false;
        player.isPastApexThreshold = false;
        player.holdingCounter = 0f;
        player.CoyoteCounterZero();
        player.ClearYVelocity();
        player.thisRB.AddForce(Vector2.up * player.jumpForce, ForceMode2D.Impulse);
        player.thisPR.LeaveGround();
        //Debug.Log(thisRB.velocity.y);
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
        base.CurrentStateCandoChange();
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        //才跳跃，操作逻辑上来看不可能马上又能跳跃的，所以并不刷新操作相关的脱台跳时间
        //但是有可能吃到一些道具，使得实现可以无限跳跃，所以依旧要判断是否能跳跃
        player.WhetherCanJumpOrWallJump();
        player.canAttack = true;

    }

    private void RiseEnter()
    {
        //if (player.keepInertia)
        //{
        //    player.thisPR.GravityLock(player.thisPR.peakGravity);
        //}
        player.thisBoxCol.enabled = false;

        player.horizontalMoveSpeedAccleration = player.airmoveAccleration;
        player.horizontalmoveThresholdSpeed = player.airmoveThresholdSpeed;
        player.horizontalMoveSpeedMax = player.airmoveSpeedMax;
        player.verticalFallSpeedMax = player.airFallSpeedMax;

    }

    private void WhetherExit()//
    {
        //if (player.thisPR.IsOnFloored())
        //{
        //    Debug.Log("???");
        //    player.ChangeToIdleState();
        //}
        //else
        if (player.thisPR.IsHead())//分开的主要目的还是考虑到可能会有不同处理，比如添加peak状态后
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
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanAttack();

    }

    public void HorizontalMove()
    {
        if (player.isGameplay)
        {
            if (player.isUncontrol)//受限移动时候的移动
            {
                //
            }
            else//非限制移动时的移动
            {
                if (player.horizontalInputVec != 0)//有键盘输入时
                {
                    if (player.faceDir != player.horizontalInputVec)//输入与人物朝向反向时，直接消除原先的速度，朝输入方向开始移动
                    {
                        player.ClearXVelocity();
                        player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalmoveThresholdSpeed, 0f);
                    }
                    else//输入与人物朝向同向时，根据当前速度不同进行启动、加速、满速移动
                    {
                        if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalMoveSpeedMax)
                        {
                            if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed)
                            {
                                //当前速度小于启动速度，则以启动速度移动
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalmoveThresholdSpeed, 0f);
                            }
                            else
                            {
                                //当前速度大于启动速度小于满速，则加速移动
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedAccleration, 0f);
                            }
                        }
                        else
                        {
                            //当前速度超过满速，则满速前进
                            player.ClearXVelocity();
                            player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedMax, 0f);
                        }
                    }
                }
                else//无键盘输入时，根据当前速度不同进行减速、停止
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
}
