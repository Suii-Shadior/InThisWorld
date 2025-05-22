using PlatformFactoryRelated;
using PlatformInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace PlatformFactoryRelated
{

    public class SensorFactory : PlatformFactory
    {
        public override IPlatform CreatePlatform(PlatformController context)
        {
            return new Sensor_Platform(context);
        }
    }


    public class Sensor_Platform : IPlatform
    {
        private readonly PlatformController _context;
        public Sensor_Platform(PlatformController context)
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
                    HideThisPlatform();
                    _context.canDisappearOrReappear = false;
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
                    ReappearThisPlatform();
                    _context.needToReappear = false;
                    _context.canDisappearOrReappear = true;
                }
            }
            else
            {
                //Debug.Log("感应中");
            }
        }

        public void Interactive_Sensor()//TODO：优化
        {
            DisappearCount();
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
            _context.canBeHaulted = true;
            _context.theStartPoint.transform.parent = null;
            _context.theEndPoint.transform.parent = null;
            _context.theNowPoint.transform.parent = null;
            _context.theDestinalPoint.transform.parent = null;
            _context.theRotatePovit_ElevatorPoint.transform.parent = null;
        }

        public void Interact1()
        {
            Interactive_Sensor();
        }

        public void Interact2()
        {
            
        }


        #region 小方法
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
        #endregion
    }
}
    
