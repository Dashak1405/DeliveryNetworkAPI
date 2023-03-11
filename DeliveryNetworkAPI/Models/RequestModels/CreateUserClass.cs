using System.ComponentModel.DataAnnotations;

namespace DeliveryNetworkAPI.Models.RequestModels
{
    public class CreateUserClass
    {
        [Required]
        [EmailAddress]
        public string login { get; set; }
        [Required]
        public string password { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string lastname { get; set; }
        public string passport { get; set; }
    }
}
