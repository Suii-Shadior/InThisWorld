using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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
        player.StateEndSkillFresh();
    }

    public override void Update()
    {
        base.Update();
        //KeepInertiaCount();
        Move();
        WhetherExit();
    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.canJumpCounter = player.canJumpLength;
        player.canWallJump = false;
        player.WhetherCanHold();
        player.canWallFall = false;
        player.canAttack = true;
        player.canCooldown = true;
    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanHold();
        player.WhetherCanJump();
        player.WhetherCanWallFall();
        player.WhetherCanDash();
    }


    private void Move()//标准？
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

    private void MoveEnter()
    {
        if (player.keepInertia)
        {
            player.thisPR.GravityLock(player.thisPR.peakGravity);
        }
        player.horizontalMoveSpeed = player.normalmoveSpeed;
        player.horizontalMoveSpeedMax = player.normalmoveSpeedMax;
        player.horizontalmoveThresholdSpeed = player.normalmoveThresholdSpeed;
    }

    private void WhetherExit()
    {
        if (!player.thisPR.IsOnGround())
        {
            player.ChangeToAirState();
        }
        else if(Mathf.Abs(player.thisRB.velocity.x) < 0.1)
        {
            player.ChangeToIdleState();
        }
    }
    private void KeepInertiaCount()
    {
        if (player.keepInertia)
        {
            player.InertiaXVelocity();
            if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalMoveSpeedMax + 0.1f)
            {
                player.thisPR.GravityUnlock();
                player.keepInertia = false;
            }
            //if (thePlayer.keepInertiaCounter >= 0)
            //{
            //    thePlayer.keepInertiaCounter -= Time.deltaTime;

            //}
            //else
            //{
            //    thePlayer.thisPR.GravityUnlock();
            //    //thePlayer.ThirdQuaterVelocity();
            //    thePlayer.keepInertia = false;
            //}
        }
    }
}
