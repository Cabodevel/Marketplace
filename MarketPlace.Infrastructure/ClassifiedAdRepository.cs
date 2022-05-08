using Marketplace.Domain;
using Raven.Client.Documents.Session;

namespace MarketPlace.Infrastructure
{
    public class ClassifiedAdRepository : IClassifiedAdRepository
    {
        private readonly IAsyncDocumentSession _session;

        public ClassifiedAdRepository(IAsyncDocumentSession session)
            => _session = session;

        public Task Add(ClassifiedAd entity)
            => _session.StoreAsync(entity, EntityId(entity.Id));

        public Task<bool> Exists(ClassifiedAdId id)
            => _session.Advanced.ExistsAsync(EntityId(id));

        public Task<ClassifiedAd> Load(ClassifiedAdId id)
            => _session.LoadAsync<ClassifiedAd>(EntityId(id));

        private static string EntityId(ClassifiedAdId id)
            => $"ClassifiedAd/{id}";
    }
}
