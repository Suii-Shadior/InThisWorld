



namespace AttackableInterfaces
{
    public interface IPhysicalAttackable
    {
      abstract void BePhysicalAttacked(AttackArea attackArea);
    }
}
namespace InteractableInterface 
{
    public interface IInteract
    {

        void Interact();
        void SetPlayer(NewPlayerController _thePlayer);
        void ClearPlayer(NewPlayerController _thePlayer);
    }
    
    public interface ISwitch
    {
        abstract void SceneLoad_Enable();
        abstract void SceneLoad_Awake();
        abstract void SceneLoad_Start();

        abstract void SceneExist_Updata();

        abstract void Interact1();
        abstract void Interact2();

        abstract void JustTrigger();
        abstract void JustReset();
    }
    public interface IFloor
    {
        abstract void SceneLoad_Enable();
        abstract void SceneLoad_Awake();
        abstract void SceneLoad_Start();

        abstract void SceneExist_Updata();

        abstract void PlayerEnter();
        abstract void PlayerStay();
        abstract void PlayerExit();
    }
}

namespace InteractiveInterface
{
    public interface IOpener
    {
        abstract void SceneLoad_Enable();
        abstract void SceneLoad_Awake();
        abstract void SceneLoad_Start();


        abstract void BePressed();
        abstract void BePressing();
        abstract void Unpressed();
    }
}

namespace PlatformInterfaces
{
    public interface IPlatform 
    {
        abstract void SceneLoad_Enable();
        abstract void SceneLoad_Awake();
        abstract void SceneLoad_Start();

        abstract void SceneExist_Updata();

        abstract void Interact1();
        abstract void Interact2();

    }


}


