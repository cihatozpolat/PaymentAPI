using System;

namespace PaymentAPI.Requests
{
    public class PaymentRequest
    {
        public virtual int ProductId { get; set; }
        public virtual decimal Balance { get; set; }
        public virtual string Name { get; set; }
        public virtual string Currency { get; set; }


    }
}