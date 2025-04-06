using MoveInterfaces;
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
        base.Enter();
        CurrentStateCandoChange();
        MoveEnter();
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
        base.CurrentStateCandoChange();
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.canAttack = true;
    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanAttack();

    }
    private void MoveEnter()
    {
        //if (player.keepInertia)
        //{
        //    player.thisPR.GravityLock(player.thisPR.peakGravity);
        //}
        player.thisBoxCol.enabled = true;

        player.horizontalMoveSpeedAccleration = player.normalmoveAccleration;
        //player.horizontalMoveSpeed = player.normalmoveSpeed;
        player.horizontalMoveSpeedMax = player.normalmoveSpeedMax;
        player.horizontalmoveThresholdSpeed = player.normalmoveThresholdSpeed;
        HorizontalMove();
    }



    private void WhetherExit()
    {
        if (!player.thisPR.IsOnFloored())
        {
            player.thisPR.LeaveGround();
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
