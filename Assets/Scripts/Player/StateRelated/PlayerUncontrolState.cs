public class PlayerUncontrolState : PlayerState
{


    public PlayerUncontrolState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.ClearVelocity();
        player.isUncontrol = true;


    }

    public override void Exit()
    {
        base.Exit();
        player.uncontrolCounter = 0;


    }

    public override void Update()
    {
        base.Update();
        if (!player.isUncontrol)
        {
            player.StateOver();
        }
    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();

    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
    }
}
