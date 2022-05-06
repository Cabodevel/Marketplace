using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class Currency : Value<Currency>
    {
        public string? CurrencyCode { get; set; }
        public bool InUse { get; set; }
        public int DecimalPlaces { get; set; }

        public static Currency None = new Currency { InUse = false };
    }
}