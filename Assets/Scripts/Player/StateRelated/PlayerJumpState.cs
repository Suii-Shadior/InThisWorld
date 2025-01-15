using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        Move();//在跳起过程中X轴移动速度仍然保持相同的加速度
        SlowDown(); 
    }
    private void Move()
    {
        if (player.isGameplay)
        {
            if (!player.isUncontrol)
            {
                if (Mathf.Abs(player.thisRB.velocity.x + player.horizontalInputVec * player.horizontalMoveSpeed * Time.deltaTime) < player.horizontalMoveSpeedMax)//在考虑到的情况中，该方案和上一句效果相同
                {
                    if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed)
                    {
                        player.thisRB.velocity += new Vector2(player.horizontalInputVec * (player.horizontalmoveThresholdSpeed + player.horizontalMoveSpeed * Time.deltaTime), 0f);
                    }
                    else
                    {
                        player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeed * Time.deltaTime, 0f);
                    }
                }
                else
                {
                    int Temp = (player.horizontalInputVec != 0) ? ((player.horizontalInputVec == player.faceDir) ? 1 : -1) : 0;
                    if (Temp < 0)
                    {
                        player.thisRB.velocity += new Vector2(player.horizontalMoveSpeed * Temp * Time.deltaTime, 0f);
                        //Debug.Log("超速状态下减速");
                    }
                    else
                    {
                        //Debug.Log("不会再加速");
                    }
                }
            }
            else
            {
                if (Mathf.Abs(player.thisRB.velocity.x + player.horizontalInputVec * player.horizontalMoveSpeed * Time.deltaTime) < player.horizontalMoveSpeedMax)//在考虑到的情况中，该方案和上一句效果相同
                {
                    switch (player.horizontalInputVec)
                    {
                        case 0:
                            if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed)
                            {
                                player.ClearXVelocity();
                            }
                            break;
                        case 1:
                            if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed || player.horizontalInputVec != player.faceDir)
                            {
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * (player.horizontalmoveThresholdSpeed + player.horizontalMoveSpeed * Time.deltaTime), 0f);
                            }
                            else
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeed * Time.deltaTime, 0f);
                            break;
                        case -1:
                            if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed || player.horizontalInputVec != player.faceDir)
                            {
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * (player.horizontalmoveThresholdSpeed + player.horizontalMoveSpeed * Time.deltaTime), 0f);
                            }
                            else
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeed * Time.deltaTime, 0f);
                            break;
                        default:
                            Debug.Log("不应该出现这种情况");
                            break;
                    }


                }
                else
                {
                    Debug.Log("不会再加速");

                }
            }
        }
    }
    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.canWallJump = false;
        player.WhetherCanHold();
        player.canWallFall = false;
        player.canAttack = true;
        player.canCooldown = true;

    }

    private void SlowDown()
    {
        if (player.thisRB.velocity.y < -player.peakSpeed) stateMachine.ChangeState(player.airState);
        if (player.thisPR.IsOnGround())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanHold();
        player.WhetherCanJump();
        player.WhetherCanWallFall();
        player.WhetherCanDash();
    }
}
