using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class NewPlayerRunState : NewPlayerState
{
    public NewPlayerRunState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
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
        HorizontalMove();
        WhetherExit();
    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
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

    }
    private void MoveEnter()
    {
        //if (player.keepInertia)
        //{
        //    player.thisPR.GravityLock(player.thisPR.peakGravity);
        //}
        player.horizontalMoveSpeedAccleration = player.normalmoveAccleration;
        //player.horizontalMoveSpeed = player.normalmoveSpeed;
        player.horizontalMoveSpeedMax = player.normalmoveSpeedMax;
        player.horizontalmoveThresholdSpeed = player.normalmoveThresholdSpeed;
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

    private void WhetherExit()
    {
        if (!player.thisPR.IsOnGround())
        {
            player.ChangeToFallState();
        }
        else if (Mathf.Abs(player.thisRB.velocity.x) < 0.1)
        {
            player.ChangeToIdleState();
        }
    }
}
