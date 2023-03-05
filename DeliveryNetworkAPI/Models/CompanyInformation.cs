namespace DeliveryNetworkAPI.Models
{
    public class CompanyInformation
    {
        public Guid ID { get; set; }
        public string CompanyName { get; set; }
        public int NumOfEmployees { get; set; }
        public int NumOfOrders { get; set; }
        public string Address { get; set; }
    }
}
