namespace PaymentAPI.Entities
{
    public class Product
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Color { get; set; }
        public virtual int Price { get; set; }
        public virtual string CatalogName { get; set; }
        public virtual int CatalogId { get; set; }
    }
}