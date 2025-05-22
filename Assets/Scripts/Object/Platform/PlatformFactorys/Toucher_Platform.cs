using PlatformInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace PlatformFactoryRelated
{
    public class ToucherFactory : PlatformFactory
    {
        public override IPlatform CreatePlatform(PlatformController context)
        {
            return new Toucher_Platform(context);
        }
    }
    public class Toucher_Platform : IPlatform
    {
        private readonly PlatformController _context;
        public Toucher_Platform(PlatformController context)
        {
            _context = context;
        }

        public void SceneExist_Updata()
        {
            //计时部分
            if (_context.hasArrived)
            {
                if (_context.hasArrivedCounter > 0)
                {
                    if (_context.thePlayer == null)
                    {
                        _context.hasArrivedCounter -= Time.deltaTime;
                    }
                    else
                    {
                        _context.hasArrivedCounter = _context.hasArrivedDuration;
                    }
                }
                else
                {
                    SetStartDestination();
                }
            }
            else
            {
                if (_context.isDestinationOrienting)
                {
                    //Debug.Log("正常上升")
                }
                else
                {
                    //Debug.Log("正常下降")
                }
            }

            //运动判断部分
            if (!_context.isHaulted)
            {
                if (Vector2.Distance(_context.theNowPoint.position, _context.theDestinalPoint.position) < .01F)
                {
                    if (Vector2.Distance(_context.theStartPoint.position, _context.theNowPoint.position) < .01f)//回到起点
                    {
                        if (!_context.hasOringinPosed)
                        {
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
                            //Debug.Log("感应中");

                        }
                    }
                    else if (Vector2.Distance(_context.theEndPoint.position, _context.theNowPoint.position) < .01F)//到达目的地
                    {
                        HssArrivedCount();
                    }
                }
                else
                {
                    if (_context.hasArrivedCounter <= 0)
                    {
                        _context.offsetVec = Vector3.MoveTowards(_context.theNowPoint.position, _context.theDestinalPoint.position, _context.moveSpeed * Time.deltaTime) - _context.theNowPoint.position;
                        _context.theNowPoint.position += _context.offsetVec;
                        _context.transform.position += _context.offsetVec;
                        if (_context.thePlayer != null)
                        {
                            //thePlayer.ClearYVelocity();
                            _context.thePlayer.transform.position += _context.offsetVec + (Vector3)_context.thePlayer.thisRB.velocity * Time.deltaTime;
                            //Debug.Log(thePlayer.horizontalInputVec);
                            //Debug.Log(thePlayer.thisRB.velocity.x);
                        }
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
        private void SetStartDestination()
        {
            _context.hasArrived = false;
            _context.theDestinalPoint.position = _context.theStartPoint.position;
        }
        private void HssArrivedCount()
        {
            if (_context.isDestinationOrienting)
            {
                _context.hasArrived = true;
                _context.hasArrivedCounter = _context.hasArrivedDuration;
                _context.isDestinationOrienting = false;
            }
        }
        public void Interactive_Sensor()//TODO：需要优化
        {

            if (_context.canBeHaulted)
            {
                _context.isHaulted = true;
                _context.haultedCounter = _context.haultedDuration;
            }
            if (_context.hasArrived)
            {
                _context.hasArrivedCounter = _context.hasArrivedDuration;
            }
            else
            {
                _context.isDestinationOrienting = true;
                _context.theDestinalPoint.position = _context.theEndPoint.position;
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
            _context.canDisappearOrReappear = true;
        }

        public void Interact1()
        {
            Interactive_Sensor();
        }

        public void Interact2()
        {
            throw new System.NotImplementedException();
        }
    }
}
