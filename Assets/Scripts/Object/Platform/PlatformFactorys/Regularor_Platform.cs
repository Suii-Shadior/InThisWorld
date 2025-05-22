using PlatformInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlatformFactoryRelated
{
    public class RegularorFactory : PlatformFactory
    {
        public override IPlatform CreatePlatform(PlatformController context)
        {
            return new Regularor_Platform(context);
        }
    }
    public class Regularor_Platform : IPlatform
    {
        private readonly PlatformController _context;

        public Regularor_Platform(PlatformController context)
        {
            _context = context;
        }

        public void SceneExist_Updata()
        {
            if (_context.needToDisappear)
            {
                if (_context.disappearCounter > 0)
                {
                    _context.disappearCounter -= Time.deltaTime;
                }
                else
                {
                    foreach (PlatformUnit unit in _context.units)
                    {
                        unit.Hide();
                    }
                    _context.needToDisappear = false;
                    ReappearCount();
                }
            }
            else if (_context.needToReappear)
            {
                if (_context.reappearCounter > 0)
                {
                    _context.reappearCounter -= Time.deltaTime;
                }
                else
                {
                    foreach (PlatformUnit unit in _context.units)
                    {
                        unit.Appear();
                    }
                    _context.needToReappear = false;
                    DisappearCount();
                }
            }
            else
            {
                Debug.Log("”–Œ Ã‚");
            }
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
            DisappearCount();
            _context.canBeHaulted = false;
            _context.canDisappearOrReappear = true;
        }
        private void ReappearCount()
        {
            _context.needToReappear = true;
            _context.reappearCounter = _context.reappearDuration;
        }
        private void DisappearCount()
        {
            _context.needToDisappear = true;
            _context.disappearCounter = _context.disappearDuration;
        }

        public void Interact1()
        {
            throw new System.NotImplementedException();
        }

        public void Interact2()
        {
            throw new System.NotImplementedException();
        }
    }
}
