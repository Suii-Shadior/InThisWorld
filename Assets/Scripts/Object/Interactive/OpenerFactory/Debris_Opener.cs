using InteractiveFactoryRelated;
using InteractiveInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenerFactoryRelated
{
    public class DebrisFactory : OpenerFactory
    {
        public override IOpener CreateOpener(OpenerController context)
        {
            return new Debris_Opener(context);
        }
    }
    public class Debris_Opener : IOpener
    {
        private readonly OpenerController context;

        public Debris_Opener(OpenerController _context)
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
            //空内容
        }

        public void SceneLoad_Awake()
        {
            throw new System.NotImplementedException();
        }

        public void SceneLoad_Enable()
        {
            context.SetAnimCont(context.theInteractiveConfig.theDebrisAnimator);
            //此处考虑优化
            context.ColOn();
            context.SetAnimUnpressed();
        }

        public void SceneLoad_Start()
        {
            throw new System.NotImplementedException();
        }

        public void Unpressed()
        {
            //空内容
        }
    }
}
