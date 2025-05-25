using AttackableInterfaces;
using UnityEngine;


namespace IPhysicalAttackableFactoryRelated
{
    public class ElevatorCallerFactory : LevelerFactory
    {
        public override IPhysicalAttackable CreateLeveler(LevelerController context)
        {
            return new ElevatorCaller_Leveler(context);
        }
    }
    public class ElevatorCaller_Leveler : IPhysicalAttackable
    {
        private readonly LevelerController _context;
        public ElevatorCaller_Leveler(LevelerController context)
        {
            _context = context;
        }

        public void BePhysicalAttacked(AttackArea attackArea)
        {
            if (_context.canBeInteracted)
            {
                if (attackArea.thePlayer.transform.position.x > _context.transform.position.x)
                {
                    //Debug.Log("����һ��������");
                    ElevatorUpwardMove();
                }
                else
                {
                    //Debug.Log("�����������ز�");
                    ElevaterDownwardMove();
                }
            }
        }
        private void ElevatorUpwardMove()
        {
            if (Vector2.Distance(_context.elevatorPlatform.theNowPoint.position, _context.elevatorPlatform.theEndPoint.position) < .01F)
            {
                //Debug.Log("��������");
            }
            else if (Vector2.Distance(_context.elevatorPlatform.theNowPoint.position, _context.elevatorPlatform.theRotatePovit_ElevatorPoint.position) < .01F)
            {
                _context.elevatorPlatform.theDestinalPoint.position = _context.elevatorPlatform.theEndPoint.position;
            }
            else if (Vector2.Distance(_context.elevatorPlatform.theNowPoint.position, _context.elevatorPlatform.theStartPoint.position) < .01F)
            {
                _context.elevatorPlatform.theDestinalPoint.position = _context.elevatorPlatform.theRotatePovit_ElevatorPoint.position;
            }
        }
        private void ElevaterDownwardMove()
        {
            if (Vector2.Distance(_context.elevatorPlatform.theNowPoint.position, _context.elevatorPlatform.theEndPoint.position) < .01F)
            {
                _context.elevatorPlatform.theDestinalPoint.position = _context.elevatorPlatform.theRotatePovit_ElevatorPoint.position;
            }
            else if (Vector2.Distance(_context.elevatorPlatform.theNowPoint.position, _context.elevatorPlatform.theRotatePovit_ElevatorPoint.position) < .01F)
            {
                _context.elevatorPlatform.theDestinalPoint.position = _context.elevatorPlatform.theStartPoint.position;
            }
            else if (Vector2.Distance(_context.elevatorPlatform.theNowPoint.position, _context.elevatorPlatform.theStartPoint.position) < .01F)
            {
                //Debug.Log("��������");
            }
        }
    }
}

