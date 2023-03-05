namespace DeliveryNetworkAPI.Models
{
    public class Products
    {
        public Guid ID { get; set; }
        public string ProductName { get; set; }
        public Manufactor Manufactor { get; set; }
        public int Count { get; set; }
    }
}
