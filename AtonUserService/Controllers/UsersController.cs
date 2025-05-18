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
            if (await usersRepository.IsRevoked(loginDto.Login)) return NotFound();

            var user = await usersRepository.Login(loginDto);
            if (user == null) return Unauthorized("Некорретный логин и/или пароль");

            var loggedUser = automapper.Map<LoggedUserDto, Users>(user);
            loggedUser.Token = tokenService.CreateToken(user);
            return Ok(loggedUser);
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
            if (await usersRepository.IsRevoked(login)) return NotFound();

            var modifierLogin = User.FindFirstValue(ClaimTypes.GivenName);
            var updatedUser = await usersRepository.UpdateData(login, updateUserDto, modifierLogin);

            if (updatedUser == null) return NotFound();

            return Ok(automapper.Map<UsersDto, Users>(updatedUser));
        }

        [HttpPut("update-password{login}")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromRoute] string login, [FromBody] string password)
        {
            if ((User.FindFirstValue(ClaimTypes.GivenName) != login) && (User.FindFirstValue(ClaimsIdentity.DefaultRoleClaimType) != "Admin")) return Forbid();
            if (await usersRepository.IsRevoked(login)) return NotFound();

            var modifierLogin = User.FindFirstValue(ClaimTypes.GivenName);
            var updatedUser = await usersRepository.UpdatePassword(login, password, modifierLogin);

            if (updatedUser == null) return NotFound();

            return Ok(automapper.Map<UsersDto, Users>(updatedUser));
        }

        [HttpPut("update-login{login}")]
        [Authorize]
        public async Task<IActionResult> UpdateLogin([FromRoute] string login, [FromBody] string new_login)
        {
            if ((User.FindFirstValue(ClaimTypes.GivenName) != login) && (User.FindFirstValue(ClaimsIdentity.DefaultRoleClaimType) != "Admin")) return Forbid();
            if (await usersRepository.IsRevoked(login)) return NotFound();
            if (!(await usersRepository.CheckLogin(new_login))) return BadRequest("Пользователь с данным логином уже существует");

            var modifierLogin = User.FindFirstValue(ClaimTypes.GivenName);
            var updatedUser = await usersRepository.UpdateLogin(login, new_login, modifierLogin);

            if (updatedUser == null) return NotFound();

            return Ok(automapper.Map<UsersDto, Users>(updatedUser));
        }

        [HttpGet("active-users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActiveUsers()
        {
            var users = await usersRepository.GetActiveUsers();

            var usersDto = users.Select(u => automapper.Map<ActiveUsersDto, Users>(u)).ToList();
            
            return Ok(usersDto);
        }

        [HttpGet("{login}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByLogin([FromRoute] string login)
        {
            var user = await usersRepository.GetUserByLogin(login);

            if (user == null) return NotFound();

            var userDto = automapper.Map<InfoUserDto, Users>(user);
            userDto.IsActive = user.RevokedOn == null;
            return Ok(userDto);
        }

        [HttpGet("aged-users{age:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersAboveAge([FromRoute] int age)
        {
            if ((age < 0) || (age > 120)) return BadRequest("Возраст должен быть от 1 до 120");

            var users = await usersRepository.GetUsersAboveAge(age);

            var usersDto = users.Select(u => automapper.Map<UsersDto, Users>(u)).ToList();

            return Ok(usersDto);
        }
    }
}