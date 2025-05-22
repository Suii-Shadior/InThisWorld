



namespace AttackableInterfaces
{
    public interface IPhysicalAttackable
    {
      abstract void BePhysicalAttacked(AttackArea attackArea);
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
