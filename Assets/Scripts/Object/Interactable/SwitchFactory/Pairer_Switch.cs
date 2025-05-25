using InteractableFactoryRelated;
using InteractableInterface;
using UnityEngine;


namespace SwitchFactoryRelated
{

    public class PairerFactory : SwitchFactory
    {
        public override ISwitch CreateSwitcher(SwitchController _context)
        {
            return new Pairer_Switch(_context);
        }
    }
    public class Pairer_Switch : ISwitch
    {
        private readonly SwitchController context;
        public Pairer_Switch(SwitchController _context)
        {
            context = _context;
        }

        public void Interact1()
        {
            foreach (PlatformController _triggeredPlatform in context.triggeredPlatforms)
            {
                _triggeredPlatform.currentPlatform.Interact1();
            }
            foreach (PlatformController _triggeredPlatform in context.unTriggeredPlatforms)
            {
                _triggeredPlatform.currentPlatform.Interact2();
            }
            context.theCombineManager.SwitchsTrigger();
        }

        public void Interact2()
        {
            foreach (PlatformController _triggeredPlatform in context.triggeredPlatforms)
            {
                _triggeredPlatform.currentPlatform.Interact2();
            }
            foreach (PlatformController _triggeredPlatform in context.unTriggeredPlatforms)
            {
                _triggeredPlatform.currentPlatform.Interact1();
            }
            context.theCombineManager.SwitchsTrigger();
        }

        public void JustReset()
        {
            //暂时不用
        }

        public void JustTrigger()
        {
            if (context.isTriggered)
            {
                context.isTriggered = false;
                context.SetAnimUntriggered();
            }
            else
            {
                context.isTriggered = true;
                context.SetAnimTriggered();
            }
            context.canTriggered = false;
            context.canTriggeredCounter = context.interactableConfigSO.canTriggeredDuration;
        }

        public void SceneExist_Updata()
        {
            if (!context.canTriggered)
            {
                if (context.canTriggeredCounter > 0)
                {
                    context.canTriggeredCounter -= Time.deltaTime;
                }
                else
                {
                    context.canTriggered = true;
                }
            }
            else
            {
                //Debug.Log("正常运作");
            }
        }

        public void SceneLoad_Awake()
        {
            if (context.isPrimarySwitch)
            {
                if (context.isTriggered)
                {
                    foreach (PlatformController triggeredPlatform in context.triggeredPlatforms)
                    {
                        triggeredPlatform.isHidden = false;
                    }
                    foreach (PlatformController triggeredPlatform in context.unTriggeredPlatforms)
                    {
                        triggeredPlatform.isHidden = true;
                    }
                }
                else
                {
                    foreach (PlatformController triggeredPlatform in context.triggeredPlatforms)
                    {
                        triggeredPlatform.isHidden = true;
                    }
                    foreach (PlatformController triggeredPlatform in context.unTriggeredPlatforms)
                    {
                        triggeredPlatform.isHidden = false;
                    }
                }
            }
        }

        public void SceneLoad_Enable()
        {
            throw new System.NotImplementedException();
        }

        public void SceneLoad_Start()
        {
            if (context.isTriggered)
            {
                context.SetAnimTriggered();
            }
            else
            {
                context.SetAnimUntriggered();
            }
        }
    }

}

