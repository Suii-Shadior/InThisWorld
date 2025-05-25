using PlayerInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerHandleState : NewPlayerState
{
    public NewPlayerHandleState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        /* 
         * Work11.继承状态机进入逻辑
         * Work2.Handle进入逻辑
         * Work3.进入帧Can判断
         * 
         */
        base.Enter();
        HandleEnter();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
        HandleExit();
    }


    public override void Update()
    {
        /*
         * Work1.普通帧Can判断
         * Work2.Hnndle逻辑，根据控制台类型将玩家输入映射到对应操纵对象逻辑内容
         * Work3.退出逻辑检测
         */
        base.Update();
        CurrentStateCandoUpdate();
        player.theHandle.HandlerUpdate();
        WhetherExit();


    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

    }





    protected override void CurrentStateCandoChange()
    {
        /* 本方法用于进行一次状态下Can判断进入变化，适用于进入帧
         * 
         * Work1.进入Handle时，不能转向
         * Work2.进入Handle时，不能水平位移、垂直位移
         * Work3.进入Handle时，默认还是在地面上，所以刷新跳跃，并进行跳跃判定
         * Work4.进入Handle时，进行Cand道具使用判断//TODO：可能还是要改，因为这种状态下进行特定道具使用感觉可以简化操作，提升玩家体验
         * Work5.进入Handle时，进行Can交互判断
         * 
         */
        base.CurrentStateCandoChange();//空内容
        player.canTurnAround = false;
        player.canHorizontalMove = false;
        player.canVerticalMove = false;
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();
    }

    protected override void CurrentStateCandoUpdate()
    {
        /* 本方法用于持续的进行Can判断，适用于普通帧
         * 
         * Work1.Handle阶段时，时刻判断刷新跳跃，并进行跳跃判定
         * Work2.Handle阶段时，默认还是在地面上，所以刷新跳跃，并进行跳跃判定
         * Work3.Handle阶段时，虽然输入上不能直接进行交互动作，但是Can道具交互还是要做的//TODO：可能还是要改，因为这种状态下进行特定道具使用感觉可以简化操作，提升玩家体验
         * Work4.Handle阶段时，进行Can交互判断
         * 
         * 
         */
        base.CurrentStateCandoUpdate();//空内容
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();
    }


    private void HandleEnter()
    {
        player.thisBoxCol.enabled = true;//TODO：这个的目的主要是为了配合下落取消物理碰撞盒的bug，迟早要改，其他所有的启用都是类似的
    }


    #region 接口实现


    #endregion
    private void WhetherExit()
    {
        /* 
         * Work1.handle =>fall
         */
        if (!player.thisPR.IsOnFloored())
        {
            player.thisPR.LeaveFloor();
            player.ChangeToFallState();
            return;
        }

    }

    private void HandleExit()
    {
        player.theHandle.ClearInput();
    }

}
