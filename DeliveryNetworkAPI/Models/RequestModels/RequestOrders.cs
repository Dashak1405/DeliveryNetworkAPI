namespace DeliveryNetworkAPI.Models.RequestModels
{
    public class RequestOrders
    {
        public Guid ID { get; set; }
        public string Customer { get; set; }
        public string Address { get; set; }
        public string Products { get; set; }
        public string? Executor { get; set; }
        public string Status { get; set; }

    }
}
