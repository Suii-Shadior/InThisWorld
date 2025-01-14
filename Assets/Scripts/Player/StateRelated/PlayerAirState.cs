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
        KeepInertiaCount();
        Move();
        Fall();


    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canBabble = true;
        player.canCooldown = true;

    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
    }

    private void AirEnter()
    {
        if (player.keepInertia)
        {
            player.thisPR.GravityLock(player.thisPR.peakGravity);
        }
        player.moveSpeed = player.airmoveSpeed;
        player.moveSpeedMax = player.airmoveSpeedMax;
        player.fallSpeedMax = player.airFallSpeedMax;

    }

    private void Move()
    {

        if (player.isGameplay && !player.isUncontrol)
        {
            float Temp = (player.horizontalInputVec != 0) ? ((player.horizontalInputVec == player.faceDir) ? 1 : -1) : 0;
            if (Mathf.Abs(player.thisRB.velocity.x) <= player.moveSpeedMax)
            {
                player.thisRB.velocity += new Vector2((player.moveSpeed + Temp) * player.faceDir * Time.deltaTime, 0f);
            }
            else if (!player.keepInertia)
            {
                //Debug.Log("我必须减速");
                player.thisRB.velocity += new Vector2((player.moveSpeedMax + Temp) * player.faceDir - player.thisRB.velocity.x, 0f);
            }
        }

        if (player.thisPR.isWall)
        {
            //Debug.Log("掉头");
            player.TurnAround();
        }
    }
    public void Fall()
    {

        if (player.thisRB.velocity.y < -player.fallSpeedMax)
        {
            player.thisRB.velocity += new Vector2(0, -player.fallSpeedMax - player.thisRB.velocity.y);
        }
        if (player.thisPR.isGround)
        {
            stateMachine.ChangeState(player.moveState);
        }
    }

    private void KeepInertiaCount()
    {
        if (player.keepInertia)
        {
            player.InertiaXVelocity();

            if (Mathf.Abs(player.thisRB.velocity.x) < player.moveSpeedMax + 0.1f)
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
