using Marketplace.Framework;
using static Marketplace.Domain.Events;

namespace Marketplace.Domain
{
    public class ClassifiedAd : AggregateRoot<ClassifiedAdId>
    {
        public ClassifiedAdId Id { get; private set; }
        public UserId OwnerId { get; private set; }
        public ClassifiedAdTitle Title { get; private set; }
        public ClassifiedAdText Text { get; private set; }
        public Price Price { get; private set; }
        public ClassifiedAdState State { get; private set; }
        public UserId ApprovedBy { get; private set; }
        public List<Picture> Pictures { get; set; }

        public ClassifiedAd(ClassifiedAdId id, UserId ownerId)
        {
            Pictures = new List<Picture>();
            Apply(new Events.ClassifiedAdCreated
            {
                Id = id,
                OwnerId = ownerId
            });
        }

        public void SetTitle(ClassifiedAdTitle title) =>
            Apply(new ClassifiedAdTitleChanged
            {
                Id = Id,
                Title = title
            });

        public void UpdateText(ClassifiedAdText text) =>
            Apply(new ClassifiedAdTextUpdated
            {
                Id = Id,
                AdText = text
            });

        public void UpdatePrice(Price price) =>
            Apply(new ClassifiedAdPriceUpdated
            {
                Id = Id,
                Price = price.Amount,
                CurrencyCode = price.Currency.CurrencyCode
            });

        public void RequestToPublish() =>
            Apply(new ClassidiedAdSentForReview { Id = Id });

        public void AddPicture(Uri pictureUri, PictureSize size) => Apply(new Events.PictureAddedToAClassifiedAd
        {
            PictureId = new Guid(),
            ClassifiedAdId = Id,
            Url = pictureUri.ToString(),
            Height = size.Height,
            Width = size.Width,
            Order = Pictures.Max(x => x.Order)
        });

        protected override void When(object @event)
        {
            switch (@event)
            {
                case ClassifiedAdCreated e:
                    Id = new ClassifiedAdId(e.Id);
                    OwnerId = new UserId(e.OwnerId);
                    State = ClassifiedAdState.Inactive;
                    break;
                case ClassifiedAdTitleChanged e:
                    Title = new ClassifiedAdTitle(e.Title);
                    break;
                case ClassifiedAdTextUpdated e:
                    Text = new ClassifiedAdText(e.AdText);
                    break;
                case ClassifiedAdPriceUpdated e:
                    Price = new Price(e.Price, e.CurrencyCode);
                    break;
                case ClassidiedAdSentForReview _:
                    State = ClassifiedAdState.PendingReview;
                    break;
                case Events.PictureAddedToAClassifiedAd e:
                    var picture = new Picture(Apply);
                    ApplyToEntity(picture, e);
                    Pictures.Add(picture);
                    break;
            }
        }

        public void ResizePicture(PictureId pictureId, PictureSize newSize)
        {
            var picture = FindPicture(pictureId);
            if (picture is null)
                throw new InvalidOperationException("Cannot resize a picture that I don't have");

            picture.Resize(newSize);
        }

        private Picture? FindPicture(PictureId id) => Pictures.FirstOrDefault(x => x.Id == id);

        private Picture? FirstPicture => Pictures.OrderBy(x => x.Order).FirstOrDefault();

        protected override void EnsureValidState()
        {
            var valid =
                Id is not null &&
                OwnerId is not null &&
                (State switch
                {
                    ClassifiedAdState.PendingReview =>
                        Title is not null
                        && Text is not null
                        && Price?.Amount > 0
                        && FirstPicture.HasCorrectSize(),
                    ClassifiedAdState.Active =>
                        Title is not null
                        && Text is not null
                        && Price?.Amount > 0
                        && FirstPicture.HasCorrectSize()
                        && ApprovedBy != null,
                    _ => true
                });

            if (!valid)
                throw new InvalidEntityStateException(this, $"Post-checks failed in state {State}");
        }
      
        public enum ClassifiedAdState
        {
            PendingReview,
            Active,
            Inactive,
            MarkedAsSold
        }
    }
}