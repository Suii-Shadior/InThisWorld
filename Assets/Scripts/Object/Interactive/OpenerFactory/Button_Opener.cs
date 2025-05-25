using InteractiveFactoryRelated;
using InteractiveInterface;
using StructForSaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OpenerFactoryRelated
{
    public class ButtonFactory : OpenerFactory
    {
        public override IOpener CreateOpener(OpenerController context)
        {
            return new Button_Opener(context);
        }
    }

    public class Button_Opener : IOpener
    {
        private readonly OpenerController context;

        public Button_Opener(OpenerController _context)
        {
            context = _context;
        }
        public void BePressed()
        {
            if (!context.isPressed)
            {
                context.isPressed = true;
                context.SetAnimPressing();
                context.theDoor.OpenTheDoor();
            }
        }

        public void BePressing()
        {
            //������
        }


        public void SceneLoad_Awake()
        {
            //������
        }

        public void SceneLoad_Enable()
        {
            context.SetAnimCont(context.theInteractiveConfig.theButtonAnimator);
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
            //������
        }

        public void Unpressed()
        {
            //������
        }
    }
}
