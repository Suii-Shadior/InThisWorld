using PlatformInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlatformFactoryRelated
{
    public class PairerFactory : PlatformFactory
    {
        public override IPlatform CreatePlatform(PlatformController context)
        {
            return new Pairer_Platform(context);
        }
    }

    public class Pairer_Platform : IPlatform
    {
        private readonly PlatformController _context;

        public Pairer_Platform(PlatformController context)
        {
            _context = context;
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
            throw new System.NotImplementedException();
        }
        public void HideThisPlatform()
        {
            foreach (PlatformUnit unit in _context.units)
            {
                unit.Hide();
            }
        }

        public void ReappearThisPlatform()
        {
            foreach (PlatformUnit unit in _context.units)
            {
                unit.Appear();
            }
        }

        public void Interact1()
        {
            HideThisPlatform();
        }

        public void Interact2()
        {
            ReappearThisPlatform();
        }
    }
}
