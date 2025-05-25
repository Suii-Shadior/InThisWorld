using InteractableFactoryRelated;
using InteractableInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FloorFactoryRelated 
{
    public class BreakerFactory : FloorFactory
    {
        public override IFloor CreateFloor(FloorController context)
        {
            throw new System.NotImplementedException();
        }
    }
    public class Breaker_Floor : IFloor
    {
        private readonly FloorController context;

        public Breaker_Floor(FloorController _context)
        {
            context = _context;
        }

        public void PlayerEnter()
        {
            if (context.thePlayer.thisRB.velocity.y < -context.breakThreshold)
            {
                context.needToDestroy--;
                if (context.needToDestroy == 0)
                {

                    context.StartCoroutine(DestoryThisFloor());
                }
            }
        }

        public void PlayerExit()
        {

        }

        public void PlayerStay()
        {
            if (context.hasDetroyed)
            {
                context.ColOff();
                context.needToDestroy = 0;
            }
            else
            {
                context.ColOn();
                context.needToDestroy = context.numToDestroy;
            }
        }

        public void SceneExist_Updata()
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        private IEnumerator DestoryThisFloor()

        {
            //第一段动画

            yield return new WaitForSeconds(context.breakDuration1);
            //第二段动画

            yield return new WaitForSeconds(context.breakDuration2);
            //第三段动画

            context.hasDetroyed = true;
            context.gameObject.SetActive(false);
        }
    }
}
