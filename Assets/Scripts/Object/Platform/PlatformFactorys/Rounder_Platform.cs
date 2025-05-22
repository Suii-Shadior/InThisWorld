using PlatformInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


namespace PlatformFactoryRelated
{
    public class RounderFactory : PlatformFactory
    {
        public override IPlatform CreatePlatform(PlatformController context)
        {
            return new Rounder_Platform(context);
        }
    }

    public class Rounder_Platform : IPlatform
    {
        private readonly PlatformController _context;

        public Rounder_Platform(PlatformController context)
        {
            _context = context;
        }

        public void SceneExist_Updata()
        {
            if (!_context.isHaulted)
            {
                if (Vector2.Distance(_context.theNowPoint.position, _context.theDestinalPoint.position) < .01F)
                {
                    if (Vector2.Distance(_context.theStartPoint.position, _context.theNowPoint.position) < .01f)
                    {
                        _context.theDestinalPoint.position = _context.theEndPoint.position;
                    }
                    else if (Vector2.Distance(_context.theEndPoint.position, _context.theNowPoint.position) < .01F)
                    {
                        _context.theDestinalPoint.position = _context.theStartPoint.position;
                    }
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
                    else
                    {
                        //Debug.Log("ÓÐÎÊÌâ");
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
