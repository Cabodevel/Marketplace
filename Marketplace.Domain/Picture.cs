using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class Picture : Entity<PictureId>
    {
        internal PictureSize Size { get; private set; }
        internal Uri Location { get; private set; }
        internal int Order { get; private set; }
        
        public Picture(Action<object> applier) : base(applier) { }
        
        protected override void When(object @event)
        {
            switch (@event)
            {
                case Events.PictureAddedToAClassifiedAd e:
                    Id = new PictureId(e.PictureId);
                    Location = new Uri(e.Url);
                    Size = new PictureSize
                    { Height = e.Height, Width = e.Width };
                    Order = e.Order;
                    break;
                case Events.ClassifiedAdPictureResized e:
                    Size = new PictureSize { Height = e.Height, Width = e.Width };
                    break;
            }
        }

        public void Resize(PictureSize newSize) => Apply(new Events.ClassifiedAdPictureResized
        {
            PictureId = Id.Value,
            Height = newSize.Width,
            Width = newSize.Width
        });
    }

    // the identity class code is still here
    public class PictureId : Value<PictureId>
    {
        public PictureId(Guid value) => Value = value;

        public Guid Value { get; }
    }
}