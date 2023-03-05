namespace DeliveryNetworkAPI.Models
{
    public class Posts
    {
        static public readonly string USER = "user";
        static public readonly string ADMIN = "admin";
        static public readonly string DELIVERYMAN = "deliveryman";
        public Guid ID { get; set; }
        public string Post { get; set; }
    }
}
