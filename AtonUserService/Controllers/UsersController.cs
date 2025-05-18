using AtonUserService.Dtos.Users;
using AtonUserService.Interfaces;
using AtonUserService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TechProcessSupportSys.Interfaces;

namespace AtonUserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ITokenService tokenService;
        private readonly IAutomapper automapper;
        private readonly IUsersRepository usersRepository;

        public UsersController(ITokenService tokenService, IAutomapper automapper, IUsersRepository usersRepository)
        {
            this.tokenService = tokenService;
            this.automapper = automapper;
            this.usersRepository = usersRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await usersRepository.Login(loginDto);
            if (user == null) return Unauthorized("Некорретный логин и/или пароль");

            return Ok(automapper.Map<UsersDto, Users>(user));
        }

        [HttpPost("create-user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!(await usersRepository.CheckLogin(createUserDto.Login))) return BadRequest("Пользователь с данным логином уже существует");
            
            var user = automapper.Map<Users, CreateUserDto>(createUserDto);
            var creatorLogin = User.FindFirstValue(ClaimTypes.GivenName);
            user.CreatedBy = creatorLogin;
            user.ModifiedBy = creatorLogin;

            await usersRepository.Create(user);

            return Ok();
        }

        [HttpPut("update-data{login}")]
        [Authorize]
        public async Task<IActionResult> UpdateData([FromRoute] string login, [FromBody] UpdateDataUserDto updateUserDto)
        {
            if ((User.FindFirstValue(ClaimTypes.GivenName) != login) && (User.FindFirstValue(ClaimsIdentity.DefaultRoleClaimType) != "Admin")) return Forbid();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await usersRepository.IsRevoked(login)) return Forbid();
            
            var updatedUser = await usersRepository.UpdateData(login, updateUserDto);

            if (updatedUser == null) return NotFound();

            var modifierLogin = User.FindFirstValue(ClaimTypes.GivenName);
            updatedUser.ModifiedOn = DateTime.Now;
            updatedUser.ModifiedBy = modifierLogin;

            return Ok(automapper.Map<UpdatedUserDto, Users>(updatedUser));
        }

        [HttpPut("update-password{login}")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromRoute] string login, [FromBody] string password)
        {
            Console.WriteLine(login);
            Console.WriteLine(ClaimTypes.GivenName);
            if ((User.FindFirstValue(ClaimTypes.GivenName) != login) && (User.FindFirstValue(ClaimsIdentity.DefaultRoleClaimType) != "Admin")) return Forbid();
            if (await usersRepository.IsRevoked(login)) return Forbid();

            var updatedUser = await usersRepository.UpdatePassword(login, password);
            Console.WriteLine(login);
            Console.WriteLine(ClaimTypes.GivenName);

            if (updatedUser == null) return NotFound();

            var modifierLogin = User.FindFirstValue(ClaimTypes.GivenName);
            updatedUser.ModifiedOn = DateTime.Now;
            updatedUser.ModifiedBy = modifierLogin;
            return Ok(automapper.Map<UpdatedUserDto, Users>(updatedUser));
        }

        [HttpPut("update-login{login}")]
        [Authorize]
        public async Task<IActionResult> UpdateLogin([FromRoute] string login, [FromBody] string new_login)
        {
            if ((User.FindFirstValue(ClaimTypes.GivenName) != login) && (User.FindFirstValue(ClaimsIdentity.DefaultRoleClaimType) != "Admin")) return Forbid();
            if (await usersRepository.IsRevoked(login)) return Forbid();
            if (!(await usersRepository.CheckLogin(new_login))) return BadRequest("Пользователь с данным логином уже существует");

            var updatedUser = await usersRepository.UpdateLogin(login, new_login);

            if (updatedUser == null) return NotFound();

            var modifierLogin = User.FindFirstValue(ClaimTypes.GivenName);
            updatedUser.ModifiedOn = DateTime.Now;
            updatedUser.ModifiedBy = modifierLogin;

            return Ok(automapper.Map<UpdatedUserDto, Users>(updatedUser));
        }
    }
}
