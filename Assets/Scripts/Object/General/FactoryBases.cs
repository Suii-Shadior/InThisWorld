using AttackableInterfaces;
using PlayerInterfaces;

using PlatformInterfaces;
using InteractableInterface;
using InteractiveInterface;

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
    public abstract class SwitchFactory
    {
        public abstract ISwitch CreateSwitcher(SwitchController context);
    }
    
    public abstract class HandlerFactory_Handler
    {
        public abstract IHandle CreateHandler(HandlerController context);
        
    }

    public abstract class FloorFactory
    {
        public abstract IFloor CreateFloor(FloorController context);
    }

}

namespace InteractiveFactoryRelated 
{
    public abstract class OpenerFactory
    {
        public abstract IOpener CreateOpener(OpenerController context);
    }
}



namespace PlatformFactoryRelated
{
    public abstract class PlatformFactory 
    {
        public abstract IPlatform CreatePlatform(PlatformController context);
        
    }

}
