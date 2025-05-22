using PlatformFactoryRelated;
using PlatformInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


namespace PlatformFactoryRelated
{


    public class SinglerFactory : PlatformFactory
    {
        public override IPlatform CreatePlatform(PlatformController context)
        {
            return new Singler_Platform(context);
        }
    }




    public class Singler_Platform : IPlatform
    {
        private readonly PlatformController _context;

        public Singler_Platform(PlatformController context)
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
            if (!_context.isHaulted)
            {
                if (Vector2.Distance(_context.theNowPoint.position, _context.theDestinalPoint.position) < .01F)
                {
                    //当平台到达终点后，计算偏移量后加在平台本身和偏移量计算点上，实现两者的重置
                    Vector3 changeVec = _context.theStartPoint.position - _context.theNowPoint.position;
                    _context.theNowPoint.position += changeVec;
                    _context.transform.position += changeVec;
                }
                else
                {
                    _context.offsetVec = Vector3.MoveTowards(_context.theNowPoint.position, _context.theDestinalPoint.position, _context.moveSpeed * Time.deltaTime) - _context.theNowPoint.position;
                    _context.theNowPoint.position += _context.offsetVec;
                    _context.transform.position += _context.offsetVec;
                    if (_context.thePlayer != null)
                    {
                        _context.thePlayer.transform.position += _context.offsetVec + (Vector3)_context.thePlayer.thisRB.velocity * Time.deltaTime;

                    }
                }
            }
            else
            {
                if (_context.haultedCounter > 0)
                {
                    _context.haultedCounter -= Time.deltaTime;
                }
                else
                {
                    _context.isHaulted = false;
                }
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
            _context.canBeHaulted = false;
            _context.theStartPoint.transform.parent = null;
            _context.theEndPoint.transform.parent = null;
            _context.theNowPoint.transform.parent = null;
            _context.theDestinalPoint.transform.parent = null;
            _context.theRotatePovit_ElevatorPoint.transform.parent = null;
            _context.theDestinalPoint.position = _context.theEndPoint.position;
        }
    }
}
