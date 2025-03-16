using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerApexState : NewPlayerState
{
    public NewPlayerApexState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        ApexEnter();
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
        ApexCounter();
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

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();

    }

    private void ApexEnter()
    {
        player.ClearYVelocity();
        player.releaseDuringRising = true;
        player.isPastApexThreshold = false;
        player.apexCounter = player.apexDuration;
        player.horizontalmoveThresholdSpeed = player.airmoveThresholdSpeed;
        player.horizontalMoveSpeedMax = player.airmoveSpeedMax;
        player.verticalFallSpeedMax = player.airFallSpeedMax;

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
    private void Fall()
    {
        if (player.thisRB.velocity.y < -player.verticalFallSpeedMax)
        {
            player.thisRB.velocity += new Vector2(0, -player.verticalFallSpeedMax - player.thisRB.velocity.y);
        }
    }
    public void WhetherExit()
    {
        if (player.thisPR.IsOnGround())
        {
            player.ChangeToIdleState();
        }
        else if(player.apexCounter<0)
        {
            player.ChangeToFallState();
        }

    }

    private void ApexCounter()
    {
        if (player.apexCounter > 0)
        {
            player.apexCounter -= Time.fixedDeltaTime;
        }
    }
}
