using System.Diagnostics;

public class PlayerIdleState : PlayerState
{
    // Start is called before the first frame update
    public PlayerIdleState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
        player.canCooldown = true;
    }

    public override void Update()
    {
        base.Update();
        Idle();


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

    private void Idle()
    {
        if (player.canAct)
        {

            player.StateOver();
        }
    }
}
