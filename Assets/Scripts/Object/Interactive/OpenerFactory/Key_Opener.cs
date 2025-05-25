using InteractiveFactoryRelated;
using InteractiveInterface;
using StructForSaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace OpenerFactoryRelated
{
    public class KeyFactory : OpenerFactory
    {
        public override IOpener CreateOpener(OpenerController context)
        {
            return new Key_Opener(context);
        }
    }

    public class Key_Opener : IOpener
    {
        private readonly OpenerController context;

        public Key_Opener(OpenerController _context)
        {
            context = _context;
        }

        public void BePressed()
        {
            if (!context.isPressed)
            {
                context.isPressed = true;
                context.SetAnimPressing();
                context.CreateFollowKey();
            }
        }

        public void BePressing()
        {
            //¿ÕÄÚÈÝ
        }

        public void SceneLoad_Awake()
        {
            //¿ÕÄÚÈÝ
        }

        public void SceneLoad_Enable()
        {
            context.SetAnimCont(context.theInteractiveConfig.theKeyAnimator);
            ISaveable saveable = context.GetComponent<ISaveable>();
            context.RegisterSaveable(saveable);
            context.isPressed = context.thisOpenerSaveData.isPressed;
            if (context.isPressed)
            {
                context.ColOff();
                context.SetAnimPressed();
            }
            else
            {
                context.ColOn();
                context.SetAnimUnpressed();
            }
        }

        public void SceneLoad_Start()
        {
            //¿ÕÄÚÈÝ
        }

        public void Unpressed()
        {
            //¿ÕÄÚÈÝ
        }
    }
}
