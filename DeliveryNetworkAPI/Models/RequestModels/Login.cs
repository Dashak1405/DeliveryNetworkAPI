using System.ComponentModel.DataAnnotations;

namespace DeliveryNetworkAPI.Models.RequestModels
{
    public class Login
    {
        [Required]
        [EmailAddress]
        public string username { get; set; }

        [Required]
        public string password { get; set; } 
    }
}
