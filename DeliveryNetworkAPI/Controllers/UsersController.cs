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
                    fio = u.Person.Surname + " " + u.Person.Name + " " + u.Person.LastName,
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
            requestUser.fio = User.Person.Surname + " " + User.Person.Name + " " + User.Person.LastName;
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
            person.post = _context.Posts.First(x => x.Post == Posts.ADMIN);
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
            string fio = userRequest.fio;

            user.Person.Surname = fio.Substring(0, fio.IndexOf(' '));
            string fio1 = fio.Remove(0, fio.IndexOf(' ') + 1);
            user.Person.Name = fio1.Substring(0, fio1.IndexOf(' '));
            string fio2 = fio1.Remove(0, fio1.IndexOf(' ') + 1);
            user.Person.LastName = fio2;
            var posts = _context.Posts.ToList();
            var post = posts.Where(x => x.Post == userRequest.role).ToList();
            user.Person.post = post[0];
            
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
