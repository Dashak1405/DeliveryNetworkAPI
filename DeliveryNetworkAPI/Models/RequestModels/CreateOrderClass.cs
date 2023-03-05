namespace DeliveryNetworkAPI.Models.RequestModels
{
    // Зачем здесь executor?
    public class CreateOrderClass
    {
        public string products { get; set; }
        public string address { get; set; }
        public string status { get; set; }
        public string dateOfStart { get; set; }
        public string dateOfEnd { get; set; }
        public string customer { get; set; }
        public string executor { get; set; }
    }
}
