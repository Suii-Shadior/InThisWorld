using PlayerInterfaces;
using InteractableFactoryRelated;
namespace HandlerFactoryRelated
{
    public class HandlerFactory : HandlerFactory_Handler
    {
        public override IHandle CreateHandler(HandlerController context)
        {
            return new PlatformHandler(context);
        }
    }
    public class PlatformHandler : IHandle
    {
        private readonly HandlerController context;
        public PlatformHandler(HandlerController _context)
        {
            context = _context;
        }
        public void HandlerUpdate()
        {
            foreach (PlatformController _platform in context.thePlatforms)
            {
                if (!context.isMirrorInput)
                {
                    _platform.handlerInput = context.thePlayer.horizontalInputVec;

                }
                else
                {
                    _platform.handlerInput = -context.thePlayer.horizontalInputVec;

                }
            }
        }
        public void ClearInput()
        {
            foreach (PlatformController _platform in context.thePlatforms)
            {
                _platform.handlerInput = 0;
            }
        }
    }
}
