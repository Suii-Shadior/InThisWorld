using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CombineInteractableManager : MonoBehaviour
{
    #region 组件及其他
    [HideInInspector] public enum CombineInteractableType { switch_and_platform_pair, leveler_and_platform_attacker}
    #endregion
    [Header("————使用说明————\n1、选择适用的启动物类型类型，即thisCombineTpye\n2、将本脚本挂载的对象设为所有的涉及对象的父对象\n3、在数组中添加相应的对象")]
    [Space(3)]

    #region 变量
    public CombineInteractableType thisCombineTpye;
    [Header("Siwtchable Related")]//用于switchable_pair和switchable_elevator
    public SwitchController[] switchs;

    [Header("Rotateable Related")]//用于rotatable_rotater
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


    #region 用于外部调用

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
