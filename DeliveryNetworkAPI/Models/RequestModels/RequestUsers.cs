namespace DeliveryNetworkAPI.Models.RequestModels
{
    public class RequestUsers
    {
        public Guid Id { get; set; }
        public string login { get; set; }
        public string fio { get; set; }
        public string role { get; set; }
    }
}
