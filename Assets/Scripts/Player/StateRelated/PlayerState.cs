public class PlayerState
{
    #region 状态机相关
    protected PlayerStateMachine stateMachine;
    protected PlayerController player;
    #endregion
    #region 组件

    #endregion
    #region 变量
    protected string animBoolName;
    protected bool stateEnd;
    #endregion

    public PlayerState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        player.thisAC.TBool(animBoolName);
        stateEnd = false;
    }

    public virtual void Update()
    {

    }

    public virtual void Exit()
    {
        player.thisAC.FBool(animBoolName);
    }
    public void CurrentStateEnd()//用于中断可中断状态，冲刺或攻击
    {
        stateEnd = true;
    }
    protected virtual void CurrentStateCandoChange()
    {

    }
    protected virtual void CurrentStateCandoUpdate()
    {

    }
}
