using FluentNHibernate.Mapping;
using PaymentAPI.Entities;

namespace PaymentAPI.Mappings
{
    public class PaymentMap : ClassMap<Payment>
    {
        public PaymentMap()
        {
            Id(x => x.Id).Not.Nullable();
            Map(x => x.Balance).Not.Nullable();
            Map(x => x.Name).Not.Nullable();
            Map(x => x.Currency).Not.Nullable();
            Map(x => x.TransactionDate);
            Map(x => x.ExchangeRatio);
            Map(x => x.ProductPrice);
        }
    }
}