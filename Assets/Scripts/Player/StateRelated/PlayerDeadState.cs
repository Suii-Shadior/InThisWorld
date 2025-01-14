public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        DeadEnter();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
        DeadExit();
        player.canCooldown = true;
    }

    public override void Update()
    {
        base.Update();
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


    private void DeadEnter()
    {
        player.thisFX.DeadSound();
        player.thisPR.GravityLock(0);
        player.ClearVelocity();
        player.Unact(player.deadUnactDuration);
        player.DisablizeColliders();

    }

    private void Dead()
    {

    }

    private void DeadExit()
    {
        player.thisPR.GravityUnlock();
        player.AblizeColliders();
    }


}
