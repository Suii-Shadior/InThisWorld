using UnityEngine;

public class PlayerBabbleState : PlayerState
{
    public PlayerBabbleState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        BabbleEnter();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
        BabbleEnd();
        player.canCooldown = true;
    }

    public override void Update()
    {
        base.Update();
        if (player.babbleCounter > 0)
        {
            player.StateOver();
        }
        else
        {
            player.babbleCounter -= Time.deltaTime;
        }
    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canBabble = false;
        player.canCooldown = false;
    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
    }

    private void BabbleEnter()
    {
        player.ClearVelocity();
        player.thisPR.GravityLock(0);
        player.babbleCounter = player.babbleDuration;
    }

    private void BabbleEnd()
    {
        player.thisPR.GravityUnlock();
        player.canBabble = false;
        player.babbleCooldownCounter = player.babbleCooldownDuration;
    }
}
