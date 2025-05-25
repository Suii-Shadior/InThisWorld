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
         * Work11.�̳�״̬�������߼�
         * Work2.Handle�����߼�
         * Work3.����֡Can�ж�
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
         * Work1.��ͨ֡Can�ж�
         * Work2.Hnndle�߼������ݿ���̨���ͽ��������ӳ�䵽��Ӧ���ݶ����߼�����
         * Work3.�˳��߼����
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
        /* ���������ڽ���һ��״̬��Can�жϽ���仯�������ڽ���֡
         * 
         * Work1.����Handleʱ������ת��
         * Work2.����Handleʱ������ˮƽλ�ơ���ֱλ��
         * Work3.����Handleʱ��Ĭ�ϻ����ڵ����ϣ�����ˢ����Ծ����������Ծ�ж�
         * Work4.����Handleʱ������Cand����ʹ���ж�//TODO�����ܻ���Ҫ�ģ���Ϊ����״̬�½����ض�����ʹ�øо����Լ򻯲����������������
         * Work5.����Handleʱ������Can�����ж�
         * 
         */
        base.CurrentStateCandoChange();//������
        player.canTurnAround = false;
        player.canHorizontalMove = false;
        player.canVerticalMove = false;
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();
    }

    protected override void CurrentStateCandoUpdate()
    {
        /* ���������ڳ����Ľ���Can�жϣ���������ͨ֡
         * 
         * Work1.Handle�׶�ʱ��ʱ���ж�ˢ����Ծ����������Ծ�ж�
         * Work2.Handle�׶�ʱ��Ĭ�ϻ����ڵ����ϣ�����ˢ����Ծ����������Ծ�ж�
         * Work3.Handle�׶�ʱ����Ȼ�����ϲ���ֱ�ӽ��н�������������Can���߽�������Ҫ����//TODO�����ܻ���Ҫ�ģ���Ϊ����״̬�½����ض�����ʹ�øо����Լ򻯲����������������
         * Work4.Handle�׶�ʱ������Can�����ж�
         * 
         * 
         */
        base.CurrentStateCandoUpdate();//������
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();
    }


    private void HandleEnter()
    {
        player.thisBoxCol.enabled = true;//TODO�������Ŀ����Ҫ��Ϊ���������ȡ��������ײ�е�bug������Ҫ�ģ��������е����ö������Ƶ�
    }


    #region �ӿ�ʵ��


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
