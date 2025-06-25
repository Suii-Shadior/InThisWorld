using PlayerInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerSwordState_Attack : NewPlayerState,IMove_horizontally, IFall_vertically
{
    public NewPlayerSwordState_Attack(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    public override void Enter()
    {
        /* 
         * Work1.�̳�״̬�������߼�
         * Work2.Attack�����߼�
         * Work3.����֡Can�ж�
         * 
         */
        base.Enter();
        AttackEnter();
        CurrentStateCandoChange();

    }

    public override void Exit()
    {
        //Itenuse��Ϊ�����ſ�ʼ������ȴ
        base.Exit();
        AttackExit();
    }

    public override void Update()
    {
        /* 
         * 
         * Work1.��ͨ֡Can�ж�
         * Work2.Fall�߼� TODO����ʵ�漰�������жϵģ�Ӧ�÷���FixedUpdate
         * Work3.����ײ����������
         * Work3.�˳��߼����
         * 
         */
        base.Update();//������
        //KeepInertiaCount();
        CurrentStateCandoUpdate();
        Fall();
        BoxColUpdate();
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
         * Work1.����Attackʱ������ת��
         * Work2.����Attackʱ������ˮƽ�ƶ������ܴ�ֱ�ƶ�
         * Work3.����Attackʱ�����ݵ�ǰ�������ж��Ƿ�ˢ���Ӻ�����ʱ��ͬʱ�ж�Can��Ծ��
         * 
         */
        base.CurrentStateCandoChange();
        player.canTurnAround = false;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        if (player.thisPR.IsOnFloored())
        {
            player.RefreshCanJump();
        }
        player.WhetherCanJumpOrWallJump();

    }

    protected override void CurrentStateCandoUpdate()
    {
        /* ���������ڳ����Ľ���Can�жϣ���������ͨ֡
         * 
         * Work1.Attackʱ�����ݵ�ǰ�������ж��Ƿ�ˢ���Ӻ�����ʱ�����Can��Ծ
         */
        base.CurrentStateCandoUpdate();
        if (player.thisPR.IsOnFloored())
        {
            player.RefreshCanJump();
        }
        player.WhetherCanJumpOrWallJump();

    }



    private void AttackEnter()
    {
        //Debug.Log("�水��");
        player.canItemUse1 = false;

    }


    private void WhetherExit()
    {
        /* 
         * Work1.Attack=>normal
         */
        if (player.isNewAC)
        {

        }
        else
        {
            if (player.thisAC.isAttackingPlaying() && player.thisAC.thisAnim.GetCurrentAnimatorStateInfo(1).normalizedTime % 1 >= .9f)
            {
                //Debug.Log(player.thisAC.thisAnim.GetCurrentAnimatorClipInfo(1)[0].clip.name);
                player.StateOver();
            }
            else
            {
                //Debug.Log(player.thisAC.thisAnim.GetCurrentAnimatorStateInfo(1).normalizedTime % 1);
                //Debug.Log(player.thisAC.thisAnim.GetCurrentAnimatorStateInfo(1).shortNameHash);
            }

        }
    }

    #region С����
    public void BoxColUpdate()
    {
        if (player.thisRB.velocity.y < 0)
        {

            player.thisBoxCol.enabled = true;
        }
        else
        {
            player.thisBoxCol.enabled = false;

        }
    }
    private void AttackExit()
    {
        player.ItemUse1Counter = player.playerConfig.sworduse1_CooldownDuration;
        player.sword_ContinueAttackCounter = player.playerConfig.sworduse1_ContinueAttackDuration;
    }

    #endregion
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
                else//�޼�������ʱ�����ݵ�ǰ�ٶȲ�ͬ���м��١�ֹͣ
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
         * TODO: ����ϵͳ�Ĺ�֮��Ӧ��Ҫ���ֵ��������½��Ĵ���
         */

        if (player.thisRB.velocity.y < -player.verticalFallSpeedMax)
        {
            player.ClearYVelocity();
            player.thisRB.velocity += new Vector2(0, -player.verticalFallSpeedMax);
        }
        else if(player.thisRB.velocity.y <0)
        {
            //Debug.Log("??");
        }
    }
    #endregion
}
