using InteractableFactoryRelated;
using InteractableInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FloorFactoryRelated
{
    public class PiperFactory : FloorFactory
    {
        public override IFloor CreateFloor(FloorController context)
        {
            throw new System.NotImplementedException();
        }
    }
    public class Piper_Floor : IFloor
    {
        private readonly FloorController context;

        public Piper_Floor(FloorController _context)
        {
            context = _context;
        }

        public void PlayerEnter()
        {
            context.passThroughCounter = context.passThroughDuration;
        }

        public void PlayerExit()
        {
            
        }

        public void PlayerStay()
        {
            if (context.thePlayer != null)
            {

                if (context.thePlayer.verticalInputVec < 0)
                {
                    context.passThroughCounter -= Time.deltaTime;
                    if (context.passThroughCounter < 0)
                    {
                        context.StartCoroutine(Pipe());
                        context.thePlayer = null;

                    }

                }
                else
                {
                    context.passThroughCounter = context.passThroughDuration;
                }
            }
            else
            {

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
            context.ColOn();
        }

        private IEnumerator Pipe()
        {
            context.thePlayer.Unact();
            context.ColIsTrigger();

            Debug.Log("½øÈë");
            yield return new WaitForSeconds(context.passDuration);
            context.thePlayer.CanAct();
            Debug.Log("ÍË³ö");
        }


    }
}
