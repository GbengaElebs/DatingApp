using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using datingapp.api.Data;
using datingapp.api.DTO;
using datingapp.api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace datingapp.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IMapper _mapper;

        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            this._mapper = mapper;
            this._config = config;
            _repo = repo;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserForRegister userforRegister)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            userforRegister.UserName = userforRegister.UserName.ToLower();

            if (await _repo.UserExists(userforRegister.UserName))
                return BadRequest("UserName already Exists");

            // var UserToCreate = new User
            // {
            //     UserName = userforRegister.UserName
            // };
            var UserToCreate = _mapper.Map<User>(userforRegister);
            var createdUser = await _repo.Register(UserToCreate, userforRegister.Password);

            var usertoreturn = _mapper.Map<UserForDetaileddto>(createdUser);
            return CreatedAtRoute("GetUser", new {controller ="Users", id= createdUser.Id}, usertoreturn);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserForLogin userForLogin)
        {

            var userfromrepo = await _repo.Login(userForLogin.userName.ToLower(), userForLogin.password);

            if (userfromrepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier ,userfromrepo.Id.ToString()),
                new Claim(ClaimTypes.Name ,userfromrepo.UserName)


            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds

            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForList>(userfromrepo);
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }


    }

}