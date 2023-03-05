namespace DeliveryNetworkAPI.Models
{
    public class Customer
    {
        public Guid ID { get; set; }
        public Users UserID { get; set; }
        public string Address { get; set; }
    }
}
