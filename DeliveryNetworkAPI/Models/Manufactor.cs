namespace DeliveryNetworkAPI.Models
{
    public class Manufactor
    {
        public Guid ID { get; set; }
        public string ManufactorName { get; set; }
        public Guid? ProductID { get; set; }
        public string Address { get; set; }
    }
}
