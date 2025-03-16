using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerFallState : NewPlayerState,IMove_horizontally,IFall_vertically
{
    public NewPlayerFallState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    public override void Enter()
    {
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
        base.Update();
        //KeepInertiaCount();//先注释掉，可以改
        HorizontalMove();
        Fall();
        CurrentStateCandoUpdate();
        WhetherExit();

    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        //才跳跃，操作逻辑上来看不可能马上又能跳跃的，所以并不刷新操作相关的脱台跳时间
        //但是有可能吃到一些道具，使得实现可以无限跳跃，所以依旧要判断是否能跳跃
        player.WhetherCanJumpOrWallJump();
        

    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanAttack();

    }

    private void FallEnter()
    {
        //if (player.keepInertia)
        //{
        //    player.thisPR.GravityLock(player.thisPR.peakGravity);
        //}
        player.releaseDuringRising = true;
        player.horizontalMoveSpeedAccleration = player.airmoveAccleration;
        player.horizontalmoveThresholdSpeed = player.airmoveThresholdSpeed;
        player.horizontalMoveSpeedMax = player.airmoveSpeedMax;
        player.verticalFallSpeedMax = player.airFallSpeedMax;

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
                    if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed)
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
    public void WhetherExit()
    {
        if (player.thisPR.IsOnGround())
        {
            player.ChangeToIdleState();
        }
        
    }
    public void Fall()
    {
        if (player.thisRB.velocity.y < -player.verticalFallSpeedMax)
        {
            player.ClearYVelocity();
            player.thisRB.velocity += new Vector2(0, -player.verticalFallSpeedMax);
            //Debug.Log("最大速度:"+player.thisRB.velocity.y);
        }
        else
        {
            if(player.thisRB.velocity.y>0)
            Debug.Log(player.thisRB.velocity.y);
        }
    }


}
