using InteractableFactoryRelated;
using InteractableInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwitchFactoryRelated
{
    public class ElevatorSwitchFactory : SwitchFactory
    {
        public override ISwitch CreateSwitcher(SwitchController _context)
        {
            return new Elevator_Switch(_context);
        }
    }
    public class Elevator_Switch : ISwitch
    {
        private readonly SwitchController context;

        public Elevator_Switch(SwitchController _context)
        {
            context = _context;
        }

        public void Interact1()
        {
            //Debug.Log("暂时无法再交互");

        }

        public void Interact2()
        {
            //Debug.Log("电梯快来！");
            context.theElevator.CallThisPlatform(context.thisElevatorArrivalPoint);
            context.theCombineManager.SwitchsTrigger();
        }

        public void JustReset()
        {
            context.canTriggered = true;
            context.isTriggered = false;
            context.SetAnimUntriggered();
        }

        public void JustTrigger()
        {
            context.isTriggered = true;
            context.canTriggered = false;
            context.SetAnimTrigger();
        }

        public void SceneExist_Updata()
        {
            if (context.isPrimarySwitch && context.theElevator.hasArrived)
            {
                context.theCombineManager.SwitchReset();
            }
        }

        public void SceneLoad_Awake()
        {

        }

        public void SceneLoad_Enable()
        {
            throw new System.NotImplementedException();
        }

        public void SceneLoad_Start()
        {
            context.SetAnimUntriggered();
        }

    }
}
