using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CombineInteractableManager : MonoBehaviour
{
    [HideInInspector] public enum CombineInteractableType { switchplatform_pair }

    [Header("————使用说明————\n1、将本脚本挂载的对象设为所有的涉及对象的父对象\n2、选择本脚本适用的组合机关类型，即thisCombineTpye\n3、根据设计在数组中添加相应的对象")]
    public CombineInteractableType thisCombineTpye;


    [Header("Siwtchable Related")]
    public SwitchController[] switchs;






    private void Awake()
    {
        
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
            _switch.JustTrigger();
        }
    }

    #endregion
}
