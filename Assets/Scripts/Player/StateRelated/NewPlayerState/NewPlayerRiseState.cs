using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerRiseState : NewPlayerState
{
    public NewPlayerRiseState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Jump();
        CurrentStateCandoChange();
    }
    public void Jump()
    {
        player.canJumpCounter = 0f;
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
        HorizontalMove();//在跳起过程中X轴移动速度仍然保持相同的加速度
        player.IsPeak();
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
        player.canAttack = true;

    }
    private void HorizontalMove()
    {
        if (player.isGameplay)
        {
            if (!player.isUncontrol)
            {
                //
            }
            else
            {
                switch (player.horizontalInputVec)
                {

                    case 0:
                        if (Mathf.Abs(player.thisRB.velocity.x - player.faceDir * player.horizontalMoveSpeedAccleration) < player.horizontalmoveThresholdSpeed)
                        {
                            player.ClearXVelocity();
                        }
                        else
                        {
                            player.thisRB.velocity += new Vector2(-player.faceDir * player.horizontalMoveSpeedAccleration, 0f);
                        }
                        break;
                    case 1:
                        if (player.faceDir == -1)
                        {
                            player.ClearXVelocity();
                            player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalmoveThresholdSpeed, 0f);
                        }
                        else
                        {
                            if (Mathf.Abs(player.thisRB.velocity.x + player.horizontalInputVec * player.horizontalMoveSpeedAccleration) < player.horizontalMoveSpeedMax)
                            {
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedAccleration, 0f);
                            }
                            else
                            {
                                player.ClearXVelocity();
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedMax, 0f);
                            }
                        }
                        break;
                    case -1:
                        if (player.faceDir == 1)
                        {
                            player.ClearXVelocity();
                            player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalmoveThresholdSpeed, 0f);
                        }
                        else
                        {
                            if (Mathf.Abs(player.thisRB.velocity.x + player.horizontalInputVec * player.horizontalMoveSpeedAccleration) < player.horizontalMoveSpeedMax)
                            {
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedAccleration, 0f);
                            }
                            else
                            {
                                player.ClearXVelocity();
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedMax, 0f);
                            }
                        }
                        break;
                }
            }
        }
    }

    private void WhetherExit()//
    {
        if (player.thisRB.velocity.y < -player.peakSpeed)
        {
            player.ChangeToFallState();
        }
        else if (player.thisPR.IsOnGround())
        {
            player.ChangeToIdleState();
        }
    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();

    }
}
