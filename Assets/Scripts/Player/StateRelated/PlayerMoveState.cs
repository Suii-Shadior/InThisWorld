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
        KeepInertiaCount();
        Move();
        isAir();
    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
    }




    private void MoveEnter()
    {
        if (player.keepInertia)
        {
            player.thisPR.GravityLock(player.thisPR.peakGravity);
        }
        player.moveSpeed = player.normalmoveSpeed;
        player.moveSpeedMax = player.normalmoveSpeedMax;
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
                //Debug.Log("ÎÒ±ØÐë¼õËÙ");
                player.thisRB.velocity += new Vector2((player.moveSpeedMax + Temp) * player.faceDir - player.thisRB.velocity.x, 0f);
            }
        }
        if (player.thisPR.isWall)
        {
            player.TurnAround();
        }
    }
    private void isAir()
    {
        if (!player.thisPR.isGround)
        {
            stateMachine.ChangeState(player.airState);
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
