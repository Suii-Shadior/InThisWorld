namespace PlayerInterfaces
{
   public interface IFall_vertically
    {
        void Fall();
    }

   public interface IMove_horizontally
    {
        void HorizontalMove();
    }
    public interface IJump 
    {
        void Jump();
    }
    public interface IInteract
    {

        void Interact();
    }
    public interface IHandle 
    {
        void HandlerUpdate();
        void ClearInput();
    }

}
