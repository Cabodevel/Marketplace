namespace Marketplace.Api
{
    public class RetryingCommandHandler<T> : IHandleCommand<T>
    {
        static RetryPolicy _policy = Policy
              .Handle<InvalidOperationException>()
              .Retry();

        private IHandleCommand<T> _next;

        public RetryingCommandHandler(IHandleCommand<T> next)
            => _next = next;

        public Task Handle(T command)
            => _policy.ExecuteAsync(() => _next.Handle(command));
    }
}
