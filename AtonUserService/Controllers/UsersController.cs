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

            return Ok(
                new UsersDto
                {
                    Login = user.Login,
                    Name = user.Name,
                    Admin = user.Admin,
                    Token = tokenService.CreateToken(user)
                });
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
            var creatorLogin = User.FindFirstValue(ClaimTypes.GivenName);
            var user = automapper.Map<Users, CreateUserDto>(createUserDto);
            user.CreatedBy = creatorLogin;
            user.ModifiedBy = creatorLogin;
            await usersRepository.Create(user);

            return Ok();
        }
    }
}
