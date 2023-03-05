using DeliveryNetworkAPI.Data;
using DeliveryNetworkAPI.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryNetworkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DeliveryNetworkDbContext _context;
        public ProductsController(DeliveryNetworkDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts() 
        {
            var products = _context.Products.Include(x => x.Manufactor).ToList();
            List<RequestProducts> productsList = new List<RequestProducts>();

            foreach (var product in products)
            {
                RequestProducts product1 = new RequestProducts
                {
                    ID = product.ID,
                    ProductName = product.ProductName,
                    Manufactor = product.Manufactor.ManufactorName,
                    Count = product.Count
                };
                productsList.Add(product1);
            }
            return Ok(productsList);
        }
    }
}
