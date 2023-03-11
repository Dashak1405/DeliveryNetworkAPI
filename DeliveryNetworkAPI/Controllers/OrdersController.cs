using DeliveryNetworkAPI.Data;
using DeliveryNetworkAPI.Models;
using DeliveryNetworkAPI.Models.RequestModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;

namespace DeliveryNetworkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DeliveryNetworkDbContext _context;
        public OrdersController(DeliveryNetworkDbContext context)
        {
            _context = context;
        }

        [HttpGet] 
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = _context.Orders
                .Include(x => x.Products)
                .Include(x => x.Status)
                .Include(x => x.Customer)
                .ThenInclude(u => u.UserID.Person.post)
                .Include(x => x.Delivery)
                .Include(x => x.Executor.Person.post)
                .ToList();
            var products = _context.Products.ToList();
            List<RequestOrders> reqOrders = new List<RequestOrders>();
            foreach (var order in orders)
            {
                var product = _context.Products.FirstOrDefault(x => x.ID == order.Delivery.ProductID);
                RequestOrders reqOrder = new RequestOrders();
                reqOrder.ID = order.ID;
                reqOrder.Customer = order.Customer.UserID.Person.GetFullName();
                reqOrder.Address = order.Address;
                reqOrder.Products = order.allProducts;
                reqOrder.Status = order.Status.status;
                if (order.Executor != null)
                {
                    reqOrder.Executor = order.Executor.Login;
                }

                reqOrders.Add(reqOrder);
            }
            return Ok(reqOrders);
        }

        [HttpPost] 
        public async Task<IActionResult> AddOrder([FromBody] CreateOrderClass requestOrders)
        {
            Orders order = new Orders();

            Guid id = Guid.NewGuid();

            Customer customer = new Customer();
                        
            var customerUser = _context.Users
                .Include(x => x.Person.post)
                .FirstOrDefault(x => x.Login == requestOrders.customer);

            // На каждый заказ создается уникальынй объект покупателя
            // Допускается обработка авторизованного пользователя или анонима
            if (customerUser != null)
            {
                customer.ID = new Guid();
                customer.Address = requestOrders.address;
                customer.UserID = customerUser;
            }

            // Отсутсвует обработка списка продуктов, берется какой-то из уже существующих в таблице лишь первый
            var products = requestOrders.products.Split(';');
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ID == Guid.Parse("8E423085-B2F8-4178-AA77-B91DF59B35CC"));

            // Дата доставки жестко вшита, она никак не управляемо
            Delivery delivery = new Delivery
            {
                ID = Guid.NewGuid(),
                //ProductID = product.ID,
                ProductID = Guid.NewGuid(),
                Address = requestOrders.address,
                DateOfDelivery = DateTime.Now.AddDays(5)
            };

            // Жестко зашит индетификатор статуса доставки. Лучше просто по имени или сделать тип enum
            Status status = _context.Status.FirstOrDefault(x => x.ID == Guid.Parse("0087713E-1BE2-4ACF-9C77-D898EAD9841E"));

            order.ID = id;
            order.Status = status;
            order.DateOfStartOrder = DateTime.Now;
            order.Address = requestOrders.address;
            order.Delivery = delivery;
            order.Customer = customer;
            order.allProducts = requestOrders.products;
            //order.Products.Add(product);
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        [HttpGet] 
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetOrdersForUser([FromRoute]Guid userId)
        {
            
            var orders = _context.Orders
                .Include(x => x.Products)
                .Include(x => x.Status)
                .Include(x => x.Customer)
                .ThenInclude(u => u.UserID.Person.post)
                .Include(x => x.Delivery)
                .Include(x => x.Executor.Person.post)
                .Include(x => x.Delivery)
                .Where(x => x.Customer.UserID.ID == userId)
                .ToList();
            List<RequestOrders> orders1 = new List<RequestOrders>();
            foreach (var order in orders)
            {
                
                RequestOrders order1 = new RequestOrders();
                order1.ID = order.ID;
                order1.Customer = order.Customer.UserID.Person.GetFullName();
                order1.Address = order.Address;
                order1.Products = order.allProducts;
                if (order.Executor != null)
                {
                    order1.Executor = order.Executor.Login;
                }
                order1.Status = order.Status.status;
                orders1.Add(order1);

            }
            return Ok(orders1);
        }

        [HttpGet] 
        [Route("/api/Orders/GetAllOrdersForDeliveryMan/{id:Guid}")]
        public async Task<IActionResult> GetOrdersForDeliveryMan([FromRoute]Guid id)
        {
            
            var orders = _context.Orders
                .Include(x => x.Products)
                .Include(x => x.Status)
                .Include(x => x.Customer)
                .ThenInclude(u => u.UserID.Person.post)
                .Include(x => x.Delivery)
                .Include(x => x.Executor.Person.post)
                .Where(x => x.ExecutorID == id)
                .ToList();

            List<RequestOrders> orders1 = new List<RequestOrders>();
            foreach (var order in orders)
            {
                RequestOrders order1 = new RequestOrders();
                order1.ID = order.ID; 
                order1.Customer = order.Customer.UserID.Person.GetFullName();
                order1.Address = order.Address;
                order1.Products = order.allProducts;
                if (order.Executor != null)
                {
                    order1.Executor = order.Executor.Login;
                }
                order1.Status = order.Status.status;
                orders1.Add(order1);

            }
            return Ok(orders1);
        }

        [HttpGet] 
        [Route("/api/Orders/GetAllPathsForMan/{id:Guid}")]
        public async Task<IActionResult> GetAddressForDeliveryMan([FromRoute]Guid id)
        {
            var status = await _context.Status.FirstOrDefaultAsync(x => x.status == Status.IN_PROGRESS);
            var orders = await _context.Orders.Include(x => x.Executor).
                Include(x => x.Status).Where(x => x.ExecutorID == id && x.Status.ID == status.ID).ToListAsync(); //Получение всех заказов, где id исполнителя = переданному id
            List<string> Paths = new List<string>();

            foreach (var order in orders)
            {
                Paths.Add(order.Address);
            }
            return Ok(Paths);
        }

        [HttpPut] 
        [Route("{id:Guid}")]
        public async Task<IActionResult> AddExecutor([FromRoute] Guid id, RequestOrders orderRequest)
        {
            var order = await _context.Orders
                .Include(x => x.Products)
                .Include(x => x.Status)
                .Include(x => x.Customer)
                .ThenInclude(u => u.UserID.Person.post)
                .Include(x => x.Delivery)
                .Include(x => x.Executor.Person.post)
                .FirstOrDefaultAsync(x => x.ID == id);

            if (order == null)
            {
                return NotFound();
            }

            // Получение сложных ID для статуса доставки, вместо использования пары констант и уникального имени
            Status status = _context.Status.FirstOrDefault(x => x.ID == Guid.Parse("1F5F5982-C3AC-4E24-8D77-08C841D62796"));

            Users user = await _context.Users
                .Include(x => x.Person)
                .ThenInclude(x => x.post)
                .FirstOrDefaultAsync(x => x.Login == orderRequest.Executor);

            order.Executor = user;
            order.Status = status;
          
            await _context.SaveChangesAsync();

            return Ok(order);
        }
        [HttpPut]
        [Route("/api/Orders/CompleteOrder/{id:Guid}")]
        public async Task<IActionResult> CompleteOrder([FromRoute] Guid id, RequestOrders requestOrders)
        {
     
            var order = await _context.Orders
                .Include(x => x.Products)
                .Include(x => x.Status)
                .Include(x => x.Customer)
                .ThenInclude(u => u.UserID.Person.post)
                .Include(x => x.Delivery)
                .Include(x => x.Executor.Person.post)
                .FirstOrDefaultAsync(x => x.ID == id);

            if (order == null)
            {
                return NotFound();
            }

            var status = await _context.Status.FirstOrDefaultAsync(x => x.status == "Completed");

            order.Status = status;

            await _context.SaveChangesAsync();
            return Ok(order);
        }
    }
}
