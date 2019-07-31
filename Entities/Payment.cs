using System;

namespace PaymentAPI.Entities
{
    public class Payment
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual decimal Balance { get; set; }
        public virtual DateTime TransactionDate { get; set; }
        public virtual string Currency { get; set; }
        public virtual decimal ExchangeRatio { get; set; }
        public virtual decimal ProductPrice { get; set; }
    }
}