namespace DeliveryNetworkAPI.Models
{
    public class Status
    {
        static public readonly string NEW = "New";
        static public readonly string IN_PROGRESS = "In Progress";
        static public readonly string COMPLETED = "Completed";

        public Guid ID { get; set; }
        public string status { get; set; }
    }
}
