namespace DeliveryNetworkAPI.Models.RequestModels
{
    public class RequestProducts
    {
        public Guid ID { get; set; }
        public string ProductName { get; set; }
        public string Manufactor { get; set; }
        public int Count { get; set; }
    }
}
