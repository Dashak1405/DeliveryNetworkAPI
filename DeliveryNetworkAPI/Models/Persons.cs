namespace DeliveryNetworkAPI.Models
{
    public class Persons
    {
        public string GetFullName() {
            return Surname + ' ' + Name + ' ' + LastName;
        }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string LastName { get; set; }
        public string PassportID { get; set; }
        public Posts post { get; set; }
    }
}
