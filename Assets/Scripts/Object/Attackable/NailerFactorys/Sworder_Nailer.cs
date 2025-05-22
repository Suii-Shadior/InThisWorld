using AttackableInterfaces;
using IPhysicalAttackableFactoryRelated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NailerFactoryRelated
{

    public class SworderFactory : NaelerFactory 
    {
        public override IPhysicalAttackable CreateNailer(NailerController context)
        {
            return new Sworder_Nailer(context);
        }

    }

    public class Sworder_Nailer : IPhysicalAttackable
    {
        private readonly NailerController _context;

        public Sworder_Nailer(NailerController context)
        {
            _context = context;
        }


        public void BePhysicalAttacked(AttackArea attackArea)
        {
            _context.isTriggered = true;
            _context.CollidersEnable();
            _context.AnimTriger();
            _context.theOnewayPlatform.TriggerThisPlatform();
        }
    }


}

