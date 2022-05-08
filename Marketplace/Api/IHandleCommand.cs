namespace Marketplace.Api
{
    public interface IHandleCommand<in T>
    {
        Task Handle(T command);
    }
}
