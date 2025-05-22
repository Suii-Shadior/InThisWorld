using PlatformInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;




namespace PlatformFactoryRelated
{

    public class HandlerFactory : PlatformFactory
    {
        public override IPlatform CreatePlatform(PlatformController context)
        {
            return new Handler_Platform(context);
        }
    }
    public class Handler_Platform : IPlatform
    {
        private readonly PlatformController _context;

        public Handler_Platform(PlatformController context)
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
            switch (_context.handlerInput)
            {
                case 1:
                    if (!_context.hasArrived)
                    {
                        _context.hasOringinPosed = false;
                        if (Vector2.Distance(_context.theNowPoint.position, _context.theEndPoint.position) < .01F)
                        {
                            //当平台到达终点后，计算偏移量后加在平台本身和偏移量计算点上，实现两者的重置
                            _context.offsetVec = _context.theEndPoint.position - _context.theNowPoint.position;
                            _context.theNowPoint.position += _context.offsetVec;
                            _context.transform.position += _context.offsetVec;
                            if (_context.thePlayer != null)
                            {
                                _context.thePlayer.transform.position += _context.offsetVec + (Vector3)_context.thePlayer.thisRB.velocity * Time.deltaTime;
                            }
                            _context.hasArrived = true;
                        }
                        else
                        {
                            _context.offsetVec = Vector3.MoveTowards(_context.theNowPoint.position, _context.theEndPoint.position, _context.moveSpeed * Time.deltaTime) - _context.theNowPoint.position;
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
                        Debug.Log("已经到顶了");
                    }
                    break;
                case -1:
                    if (!_context.hasOringinPosed)
                    {
                        _context.hasArrived = false;
                        if (Vector2.Distance(_context.theNowPoint.position, _context.theStartPoint.position) < .01F)
                        {
                            //当平台到达终点后，计算偏移量后加在平台本身和偏移量计算点上，实现两者的重置
                            _context.offsetVec = _context.theStartPoint.position - _context.theNowPoint.position;
                            _context.theNowPoint.position += _context.offsetVec;
                            _context.transform.position += _context.offsetVec;
                            if (_context.thePlayer != null)
                            {
                                _context.thePlayer.transform.position += _context.offsetVec + (Vector3)_context.thePlayer.thisRB.velocity * Time.deltaTime;
                            }
                            _context.hasOringinPosed = true;
                        }
                        else
                        {
                            _context.offsetVec = Vector3.MoveTowards(_context.theNowPoint.position, _context.theStartPoint.position, _context.moveSpeed * Time.deltaTime) - _context.theNowPoint.position;
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
                        Debug.Log("已经到底了");
                    }
                    break;
                case 0:
                    if (!_context.hasOringinPosed)
                    {
                        _context.hasArrived = false;
                        if (_context.perBackStepCounter >= 0)
                        {
                            _context.perBackStepCounter -= Time.deltaTime;
                        }
                        else
                        {
                            if (Vector2.Distance(_context.theNowPoint.position, _context.theStartPoint.position) < .01F)
                            {
                                //Debug.Log(Vector2.Distance(theNowPoint.position, theStartPoint.position));
                                //当平台到达终点后，计算偏移量后加在平台本身和偏移量计算点上，实现两者的重置
                                _context.offsetVec = _context.theStartPoint.position - _context.theNowPoint.position;
                                _context.theNowPoint.position += _context.offsetVec;
                                _context.transform.position += _context.offsetVec;
                                if (_context.thePlayer != null)
                                {
                                    _context.thePlayer.transform.position += _context.offsetVec + (Vector3)_context.thePlayer.thisRB.velocity * Time.deltaTime;
                                }
                                _context.hasOringinPosed = true;
                            }
                            else
                            {
                                _context.offsetVec = Vector3.MoveTowards(_context.theNowPoint.position, _context.theStartPoint.position, _context.moveSpeed * Time.deltaTime) - _context.theNowPoint.position;
                                _context.theNowPoint.position += _context.offsetVec;
                                _context.transform.position += _context.offsetVec;
                                if (_context.thePlayer != null)
                                {
                                    _context.thePlayer.transform.position += _context.offsetVec + (Vector3)_context.thePlayer.thisRB.velocity * Time.deltaTime;
                                }
                            }
                            _context.perBackStepCounter = _context.perBackStepDuration;
                        }
                    }
                    else
                    {
                        //Debug.Log("感应中");
                    }
                    break;
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
        }
    }
}
