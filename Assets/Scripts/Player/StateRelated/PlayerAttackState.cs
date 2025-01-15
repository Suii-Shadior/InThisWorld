using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    public PlayerAttackState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("¹¥»÷£¡");
        BladeAttackCounter();
        //player.BladeAtttackOnGround();

    }

    public override void Exit()
    {
        base.Exit();
        //player.BladeAttackEnd();

    }

    public override void Update()
    {
        base.Update();
        if (!stateEnd)
        {
            CurrentStateCandoUpdate();
            if (player.continueAttackCounter > 0)
            {
                player.continueAttackCounter -= Time.deltaTime;
            }
        }
        else
        {
            stateEnd = false;
            if (player.thisPR.IsOnGround())
            {
                stateMachine.ChangeState(player.idleState);
                return;
            }
            else
            {
                stateMachine.ChangeState(player.airState);
                return;
            }
        }
    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canHorizontalMove = false;
        player.canVerticalMove = false;
        player.canJumpCounter = 0;
        player.canHold = false;
        player.canWallFall = false;
        player.canAttack = false;


    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanJump();
        player.WhetherCanDash();
        player.WhetherCanHold();

    }

    private void BladeAttackCounter()
    {
        if (player.continueAttackCounter <= 0 || player.attackCounter >= 2)
        {
            player.attackCounter = 0;
        }
        else
        {
            player.attackCounter++;
        }
        player.thisAC.AttackTrigger();
    }
}
