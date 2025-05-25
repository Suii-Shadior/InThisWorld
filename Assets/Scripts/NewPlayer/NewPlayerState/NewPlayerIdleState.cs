using PlayerInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerIdleState : NewPlayerState, IMove_horizontally
{
    public NewPlayerIdleState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    public override void Enter()
    {
        /* 
         * Work1.�̳�״̬�������߼�
         * Work2.idle�����߼�
         * Work3.����֡Can�ж�
         * 
         */
        base.Enter();
        IdleEnter();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {   
        /* 
         * 
         * Work1.��ͨ֡Can�ж�
         * Work2.Fall�߼� TODO����ʵ�漰�������жϵģ�Ӧ�÷���FixedUpdate
         * Work3.�˳��߼����
         * 
         */
        base.Update();
        CurrentStateCandoUpdate();
        WhetherExit();


    }


    public override void FixedUpdate()
    {
        base.FixedUpdate();
        HorizontalMove();

    }

    protected override void CurrentStateCandoChange()
    {
        /* ���������ڽ���һ��״̬��Can�жϽ���仯�������ڽ���֡
         * 
         * Work1.����Idleʱ������ת��
         * Work2.����Idleʱ������ˮƽ�ƶ������ܴ�ֱ�ƶ�
         * Work3.����Idleʱ��ˢ���ӳ�����ʱ���ж�Can��Ծ
         * Work4.����Idleʱ������Can����ʹ���ж�
         * Work5.����Idleʱ������Can�����ж�
         * 
         */
        base.CurrentStateCandoChange();
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();
    }

    protected override void CurrentStateCandoUpdate()
    {
        /* ���������ڳ����Ľ���Can�жϣ���������ͨ֡
         * 
         * Work1.Idle�׶�ʱ��ʱ���ж�ˢ����Ծ����������Ծ�ж�
         * Work2.Idle�׶�ʱ��Ĭ�ϻ����ڵ����ϣ�����ˢ����Ծ����������Ծ�ж�
         * Work3.Idle�׶�ʱ������Can���߽����ж�
         * Work4.Idle�׶�ʱ������Can�����ж�
         * 
         * 
         */
        base.CurrentStateCandoUpdate();
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();
    }



    private void IdleEnter()
    {
        /* 
         * Step1.���Player���ٶ�,ͬʱ������ض�����ս��ж����λ�ò���
         * Steo2.�ָ���ײ��
         * Step3.player��ز����仯
         */
        player.ClearYVelocity();

        if(player.thisPR.IsOnFloored()&&!player.thisPR.WasOnFloored())
        //if (player.isStandOnPlatform() && !player.thisPR.wasFloored) 
        {
            if (player.isStandOnPlatform())
            //Debug.Log("����ƽ̨");
            player.transform.position -=new Vector3(0, player.thisPR.RayHit().distance,0);
        }
        player.thisBoxCol.enabled = true;


        player.horizontalMoveSpeedAccleration = player.playerConfig.idle_MoveAccleration;
        player.horizontalMoveSpeedMax = player.playerConfig.idle_MoveSpeedMax;
        player.horizontalmoveThresholdSpeed = player.playerConfig.idle_MoveThresholdSpeed;
    }

    private void WhetherExit()
    {
        /* 
         * Work1.idle=>fall
         * Work2.idle=>horizontalMove
         * 
         * 
         */
        if (!player.thisPR.IsOnFloored())
        {
            player.thisPR.LeaveFloor();
            player.ChangeToFallState();
            return;
        }
        else
        if (player.horizontalInputVec != 0)
        {
            
            player.ChangeToHorizontalMoving();
            return;
        }
    }



    public void HorizontalMove()
    {
        if (player.GetIsUncontrol())//�����ƶ�ʱ����ƶ�
        {
            //
        }
        else
        {
            if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed||player.thisPR.IsOnWall())
            {
                //��ǰ�ٶ�С�ڵ��������ٶȣ���ֹͣ
                player.ClearXVelocity();
            }
            else
            {
                //��ǰ�ٶȴ��������ٶȣ������
                player.thisRB.velocity += new Vector2(-player.faceDir * player.horizontalMoveSpeedAccleration, 0f);
            }

        }
    }
}
