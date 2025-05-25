using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CombineInteractableManager : MonoBehaviour
{
    #region ���������
    [HideInInspector] public enum CombineInteractableType { switch_and_platform_pair, leveler_and_platform_attacker}
    #endregion
    [Header("��������ʹ��˵����������\n1��ѡ�����õ��������������ͣ���thisCombineTpye\n2�������ű����صĶ�����Ϊ���е��漰����ĸ�����\n3���������������Ӧ�Ķ���")]
    [Space(3)]

    #region ����
    public CombineInteractableType thisCombineTpye;
    [Header("Siwtchable Related")]//����switchable_pair��switchable_elevator
    public SwitchController[] switchs;

    [Header("Rotateable Related")]//����rotatable_rotater
    public LevelerController[] levelers;
    #endregion

    private void Awake()
    {
        switch (thisCombineTpye)
        {
            case CombineInteractableType.leveler_and_platform_attacker:
                levelers = GetComponentsInChildren<LevelerController>();
                break;
            case CombineInteractableType.switch_and_platform_pair:
                switchs = GetComponentsInChildren<SwitchController>();
                break;
        }
    }


    private void Start()
    {
        
    }


    private void Update()
    {
        
    }


    #region �����ⲿ����

    public void SwitchsTrigger()
    {
        foreach (SwitchController _switch in switchs)
        {
            _switch.currentSwitch?.JustTrigger(); 
        }
    }
    public void SwitchReset()
    {
        foreach (SwitchController _switch in switchs)
        {
            _switch.currentSwitch?.JustReset();
        }
    }

    public void LevelersInteract()
    {
        foreach (LevelerController _leveler in levelers)
        {
            _leveler.JustInteract();
        }
    }


    public void LevelersAltInteract()
    {
        foreach (LevelerController _leveler in levelers)
        {
            _leveler.JustAltInteract();
        }
    }

    #endregion
}
