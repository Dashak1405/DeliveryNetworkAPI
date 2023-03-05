namespace DeliveryNetworkAPI.Models
{
    public class Users
    {
        public Guid ID { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public Persons Person { get; set; }
    }
}
