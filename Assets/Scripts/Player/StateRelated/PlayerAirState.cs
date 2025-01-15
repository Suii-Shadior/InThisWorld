using Unity.VisualScripting;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        AirEnter();
        CurrentStateCandoChange();

    }

    public override void Exit()
    {
        base.Exit();
        player.StateEndSkillFresh();
    }

    public override void Update()
    {
        base.Update();
        //KeepInertiaCount();//先注释掉，可以改
        Move();
        Fall();


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

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanHold();
        player.WhetherCanJump();
        player.WhetherCanWallFall();
        player.WhetherCanDash();
    }

    private void AirEnter()
    {
        if (player.keepInertia)
        {
            player.thisPR.GravityLock(player.thisPR.peakGravity);
        }
        player.horizontalMoveSpeed = player.airmoveSpeed;
        player.horizontalMoveSpeedMax = player.airmoveSpeedMax;
        player.verticalFallSpeedMax = player.airFallSpeedMax;

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
    public void Fall()
    {

        if (player.thisRB.velocity.y < -player.verticalFallSpeedMax)
        {
            player.thisRB.velocity += new Vector2(0, -player.verticalFallSpeedMax - player.thisRB.velocity.y);
        }
        if (player.thisPR.IsOnGround())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    private void KeepInertiaCount()//TD：在Uncontrol的时候用这个比较好
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
