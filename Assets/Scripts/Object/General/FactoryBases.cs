using AttackableInterfaces;
using PlayerInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlatformInterfaces;

namespace IPhysicalAttackableFactoryRelated
{
    public abstract class LevelerFactory 
    {
        public abstract IPhysicalAttackable CreateLeveler(LevelerController context);
    }
    public abstract class BlockerFactory  
    {
        public abstract IPhysicalAttackable CreateBlocker(BlockerController context);
    }
    public abstract class NaelerFactory 
    {
        public abstract IPhysicalAttackable CreateNailer(NailerController context);
    }

}
namespace InteractableFactoryRelated
{
    public abstract class HandlerFactory 
    {
        public abstract IHandle CreateHandler(HandlerController context);
        
    }
    

}

namespace PlatformFactoryRelated
{
    public abstract class PlatformFactory 
    {
        public abstract IPlatform CreatePlatform(PlatformController context);
        
    }

}
