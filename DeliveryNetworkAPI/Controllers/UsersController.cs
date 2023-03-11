using DeliveryNetworkAPI.Data;
using DeliveryNetworkAPI.Models;
using DeliveryNetworkAPI.Models.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;

namespace DeliveryNetworkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DeliveryNetworkDbContext _context;
        public UsersController(DeliveryNetworkDbContext context)
        {
            _context = context;
        }

        [HttpGet] 
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _context.Users.Include(c => c.Person).ThenInclude(x => x.post).ToList();
            List<RequestUsers> usersList = new List<RequestUsers>();
            foreach (var u in users)
            {
                RequestUsers user = new RequestUsers
                {
                    Id = u.ID,
                    login = u.Login,
                    fio = u.Person.GetFullName(),
                    role = u.Person.post.Post
                };
                usersList.Add(user);
            };
            return Ok(usersList);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetEditUser([FromRoute]Guid id) 
        {
            var User = await _context.Users.Include(c => c.Person).ThenInclude(x => x.post).FirstOrDefaultAsync(x => x.ID == id);
            RequestUsers requestUser = new RequestUsers();
            
            if (User == null)
            {
                return NotFound();
            }

            requestUser.Id = User.ID;
            requestUser.login = User.Login;
            requestUser.fio = User.Person.GetFullName();
            requestUser.role = User.Person.post.Post;
            return Ok(requestUser);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody]CreateUserClass userRequest) 
        {
            Users user = new Users();
            Persons person = new Persons();
            user.ID = Guid.NewGuid(); 
            user.Login = userRequest.login;
            user.Password = userRequest.password;
            person.ID = Guid.NewGuid();
            person.Name = userRequest.name;
            person.Surname = userRequest.surname;
            person.LastName = userRequest.lastname;
            person.post = _context.Posts.First(x => x.Post == Posts.USER);
            person.PassportID = userRequest.passport;
            user.Person = person;
            await _context.Users.AddAsync(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict();
            }
                        
            return Created("AddUser", user.Login);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, RequestUsers userRequest) 
        {
            var user = await _context.Users.Include(x => x.Person).ThenInclude(x => x.post).FirstOrDefaultAsync(x => x.ID == id);

            if (user == null)
            {
                return NotFound();
            }
            user.Login = userRequest.login;
            var person = user.Person;
            var nameParts = userRequest.fio.Split(' ');
            person.Surname  = nameParts[0];
            person.Name     = nameParts[1];
            person.LastName = nameParts[2];
            person.post = _context.Posts.First(x => x.Post == userRequest.role);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpDelete]
        [Route("{id:Guid}")] 
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }
    }
}
