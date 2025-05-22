using PlatformInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformFactoryRelated
{
    public class PresserFactory : PlatformFactory
    {
        public override IPlatform CreatePlatform(PlatformController context)
        {
            return new Presser_Platform(context);
        }
    }
    public class Presser_Platform : IPlatform
    {
        private readonly PlatformController _context;

        public Presser_Platform(PlatformController context)
        {
            _context = context;
        }

        public void Interact1()
        {
            throw new System.NotImplementedException();
        }

        public void Interact2()
        {
            throw new System.NotImplementedException();
        }

        public void SceneExist_Updata()
        {
            //Пе
        }

        public void SceneLoad_Awake()
        {
            throw new System.NotImplementedException();
        }

        public void SceneLoad_Enable()
        {
            throw new System.NotImplementedException();
        }

        public void SceneLoad_Start()
        {
            _context.canBeHaulted = false;
            _context.canDisappearOrReappear = true;
        }
    }
}
