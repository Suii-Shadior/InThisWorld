using AttackableInterfaces;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace IPhysicalAttackableFactoryRelated 
{

    public class RotaterFactory : LevelerFactory
    {
        public override IPhysicalAttackable CreateLeveler(LevelerController _context)
        {
            return new Rotater_Leveler(_context);
        }
    }
 

    public class Rotater_Leveler : IPhysicalAttackable
    {
        private readonly LevelerController context;
        public Rotater_Leveler(LevelerController _context)
        {
            context = _context;
        }

        public void BePhysicalAttacked(AttackArea attackArea)
        {
            if (context.canBeInteracted)
            {
                if (attackArea.thePlayer.transform.position.x > context.transform.position.x)
                {
                    //Debug.Log("左手一个慢动作");
                    ClockwiseRotate();
                }
                else
                {
                    //Debug.Log("右手慢动作重播");
                    AntiClockwiseRotate();
                }
            }
        }

        private void ClockwiseRotate()
        {
            foreach (PlatformController rotatePlatform in context.rotatePlatforms)
            {
                rotatePlatform.currentPlatform.Interact1();
            }
            context.theCombineManager.LevelersInteract();
        }
        private void AntiClockwiseRotate()
        {
            foreach (PlatformController rotatePlatform in context.rotatePlatforms)
            {
                rotatePlatform.currentPlatform.Interact2();
            }
            context.theCombineManager.LevelersAltInteract();
        }
    }

  
}

