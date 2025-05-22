using PlatformInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace PlatformFactoryRelated
{

    public class RotaterFactory : PlatformFactory
    {
        public override IPlatform CreatePlatform(PlatformController context)
        {
            return new Rotater_Platform(context);
        }
    }

    public class Rotater_Platform : IPlatform
    {
        private readonly PlatformController _context;

        public Rotater_Platform(PlatformController context)
        {
            _context = context;
        }

        public void Interact1()
        {
            ClockwiseRotate();
        }

        public void Interact2()
        {
            AntiClockwiseRotate();
        }

        public void SceneExist_Updata()
        {
            if (_context.isRotating)
            {
                _context.transform.RotateAround(_context.theRotatePovit_ElevatorPoint.position, Vector3.forward, _context.isClockwise * _context.rotateStep * Time.deltaTime);
                _context.hadRotated += _context.rotateStep * Time.deltaTime;
                if (_context.hadRotated > 90f)
                {
                    _context.isRotating = false;
                    _context.nowAngle += _context.isClockwise * 90f;
                    if (_context.nowAngle < 0) _context.nowAngle += 360f;
                    if (_context.nowAngle >= 360) _context.nowAngle %= 360f;
                    _context.transform.RotateAround(_context.theRotatePovit_ElevatorPoint.position, Vector3.forward, (-_context.hadRotated + 90f) * _context.isClockwise * Time.deltaTime);
                }
            }
            else
            {
                //Debug.Log("µÈ´ýÐý×ª");
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
            _context.theRotatePovit_ElevatorPoint.transform.parent = null;
        }
        public void ClockwiseRotate()
        {
            _context.isRotating = true;
            _context.rotateStep = 90f / _context.rotationDuration;
            _context.isClockwise = 1;
            _context.hadRotated = 0;
        }

        public void AntiClockwiseRotate()
        {
            _context.isRotating = true;
            _context.rotateStep = 90f / _context.rotationDuration;
            _context.isClockwise = -1;
            _context.hadRotated = 0;
        }
    }
}
