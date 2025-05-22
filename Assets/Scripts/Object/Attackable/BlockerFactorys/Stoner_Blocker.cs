using AttackableInterfaces;
using IPhysicalAttackableFactoryRelated;
using OtherEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockerFactoryRelated
{

    public class StonerFactory : BlockerFactory
    {
        public override IPhysicalAttackable CreateBlocker(BlockerController context)
        {
            return new Stoner_Blocker(context);
        }
    }

    public class Stoner_Blocker : IPhysicalAttackable
    {
        private readonly BlockerController _context;   

        public Stoner_Blocker(BlockerController context)
        {
            _context = context;
        }
        public void BePhysicalAttacked(AttackArea attackArea)
        {

            if (_context.canBeAttackedVec2 == attackArea.AttackVec)
            {
                HitThisObstacle();
            }

            
        }
        private void HitThisObstacle()
        {
            /* 
             * 
             */
            //Debug.Log("³É¹¦");
            _context.needToTriggered--;
            _context.AnimAttacking();
            if (_context.needToTriggered == 0)
            {

                _context.canBeAttackableCounter = _context.attackConfigSO.Blocker_canBeAttackableDuration;
                _context.AnimTriggered();
                _context.CollidersEnable();
                _context.isTriggered = true;
            }
            else
            {
                _context.canBeAttackableCounter = _context.attackConfigSO.Blocker_canBeAttackableDuration;
            }

        }
    }
}
