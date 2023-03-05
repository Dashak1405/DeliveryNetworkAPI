namespace DeliveryNetworkAPI.Models
{
    public class Delivery
    {
        public Guid ID { get; set; }
        public DateTime DateOfDelivery { get; set; }
        public string Address { get; set; }
        public Guid ProductID { get; set; }
    }
}
