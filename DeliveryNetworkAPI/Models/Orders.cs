namespace DeliveryNetworkAPI.Models
{
    public class Orders
    {
        public Guid ID { get; set; }
        public Customer Customer { get; set; }
        public string Address { get; set; }
        public List<Products> Products { get; set; }
        public string allProducts { get; set; }
        public Guid? ExecutorID { get; set; }
        public Users Executor { get; set; }
        public Status Status { get; set; }
        public DateTime DateOfStartOrder { get; set; }
        public DateTime DateOfEndOrder { get; set; }
        public Delivery Delivery { get; set; }
    }
}
