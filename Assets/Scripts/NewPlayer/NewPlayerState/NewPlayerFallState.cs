using PlayerInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewPlayerFallState : NewPlayerState, IFall_vertically, IMove_horizontally
{
    public NewPlayerFallState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    public override void Enter()
    {
        /* 
         * Work1.�̳�״̬�������߼�
         * Work2.Fall�����߼�
         * Work3.����֡Can�ж�
         * 
         */
        base.Enter();
        FallEnter();
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
        base.Update();//������
        //KeepInertiaCount();//��ע�͵������Ը�
        CurrentStateCandoUpdate();
        Fall();
        WhetherExit();

    }
    public override void FixedUpdate()
    {
        /*
         * ˮƽλ���߼�
         */
        base.FixedUpdate();
        HorizontalMove();

    }

    protected override void CurrentStateCandoChange()
    {
        /* ���������ڽ���һ��״̬��Can�жϽ���仯�������ڽ���֡
         * 
         * Work1.����ʱ��Ȼ�������ҷ���
         * Work2.����ʱ��Ȼ��������λ��
         * Work3.����ʱ��������λ��
         * Work4.����ʱ���������жϣ������߼�����������������������Ծ�ģ���Ϊ�ڽ���֡�Ѿ����Ӻ���ʱ����0������ͨ���˷��������жϱ�ֱ���÷�Ҫ����һ�㡣��Ҫ�ǵ����п��ܳԵ�һЩ���ߣ�ʹ��ʵ�ֿ���������Ծ����������Ҫ�ж��Ƿ�����Ծ
         */
        base.CurrentStateCandoChange();//������
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;

        player.WhetherCanJumpOrWallJump();
        

    }

    protected override void CurrentStateCandoUpdate()
    {
        /* ���������ڳ����Ľ���Can�жϣ���������ͨ֡
         * 
         * Work1.����ʱʱ�̼��Can��Ծ
         * Work2.����ʱʱ�̼��Can����ʹ��
         * Work3.����ʱʱ�̼��Can����
         */
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();

    }

    private void FallEnter()
    {

        player.thisBoxCol.enabled = true;
        player.horizontalMoveSpeedAccleration = player.playerConfig.air_MoveAccleration;
        player.horizontalmoveThresholdSpeed = player.playerConfig.air_MoveThresholdSpeed;
        player.horizontalMoveSpeedMax = player.playerConfig.air_MoveSpeedMax;
        player.verticalFallSpeedMax = player.playerConfig.air_FallSpeedMax;

    }
    public void WhetherExit()
    {
        /* 
         * fall => idle
         */
        if (player.thisPR.IsOnFloored())
        {

            player.ChangeToIdleState();
        }
        
    }

    #region �ӿ�ʵ��
    public void HorizontalMove()
    {
        /* ���ӿڷ�������ͨ�������ʵ�ֲ�ͬ״̬�µ�ˮƽλ��
         * 
         * 
         * Step1.�ж��Ƿ���gameplay��.���ǣ�����Step2;����������
         * Step2.�ж��Ƿ�isUncontrol�����ǣ�����Step3-a�����񣬽���Step3-b��
         * Step3-a.
         * Step3-b.�����Ƿ������롢���뷽���Ƿ��뵱ǰ�泯������ͬ����ǰ��ˮƽλ���ٶȽ��в�ͬ���߼��жϡ�
         *      ���泯��������뷽��һ�¡������ٶȴ��ڵ�������ٶȣ������泯������ά������ٶȣ�
         *                                  ���ٶ�С������ٶ��Ҵ����ż��ٶȣ������泯������м��٣�
         *                                  ���ٶ�С���ż��ٶȣ������泯�������ż��ٶ��ƶ���
         *      ���泯��������뷽��һ�¡������ٶȴ����ż��ٶȣ������泯�����Ͻ��м��٣�
         *                                    ������С�ڵ����ż��ٶȣ��������뷽�����ż��ٶ��ƶ�//TODO�����������ת��������ʵӦ����ԭ��ͣ������ȴ���ͷת��
         *      �������롪�����ٶȴ����ż��ٶȣ������泯�����Ͻ��м��٣�
         *                  ������С�ڵ����ż��ٶȣ����泯�ٶ���0
         * 
         */
        if (player.GetIsGamePlay())
        {
            if (player.GetIsUncontrol())
            {

            }
            else
            {
                if (player.horizontalInputVec != 0)
                {
                    if (player.faceDir != player.horizontalInputVec)
                    {
                        player.ClearXVelocity();
                        player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalmoveThresholdSpeed, 0f);
                    }
                    else
                    {
                        if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalMoveSpeedMax)
                        {
                            if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed)
                            {
                                
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalmoveThresholdSpeed, 0f);
                            }
                            else
                            {
                                
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedAccleration, 0f);
                            }
                        }
                        else
                        {
                            player.ClearXVelocity();
                            player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedMax, 0f);
                        }
                    }
                }
                else
                {
                    if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed || player.thisPR.IsOnWall())
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
    }
    public void Fall()
    {
        /* ���ӿڷ�������ʵ�ֲ�ͬ״̬�µ���������
         * 
         * �жϵ�ǰ�ٶ��Ƿ�С�ڵ�����������ٶȡ����ǣ������䷽��������ٶ��ƶ������������ݣ�ͨ�����������䷽����м��٣�
         * TODO��������������Ӧ�������������ٶȼ���������ٶȡ�
         */

        if (player.thisRB.velocity.y < -player.verticalFallSpeedMax)
        {
            player.ClearYVelocity();
            player.thisRB.velocity += new Vector2(0, -player.verticalFallSpeedMax);
            //Debug.Log("����ٶ�:"+player.thisRB.velocity.y);
        }
        else
        {

        }
    }

    #endregion

}
