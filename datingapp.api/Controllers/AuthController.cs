using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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

        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo , IConfiguration config)
        {
            this._config=config;
            _repo=repo;
        }
        
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserForRegister userforRegister)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            userforRegister.UserName = userforRegister.UserName.ToLower();

            if(await _repo.UserExists(userforRegister.UserName))
            return  BadRequest("UserName already Exists");

            var UserToCreate= new User
            {
                UserName =userforRegister.UserName
            };

            var createdUser= await _repo.Register(UserToCreate,userforRegister.Password);
            return StatusCode(201);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserForLogin userForLogin)
        {
            var userfromrepo= await _repo.Login(userForLogin.userName.ToLower(), userForLogin.password);

            if(userfromrepo == null)
            return Unauthorized();

            var claims= new[]
            {
                new Claim(ClaimTypes.NameIdentifier ,userfromrepo.Id.ToString()),
                new Claim(ClaimTypes.Name ,userfromrepo.UserName)

                
            };

            var key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds=new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor= new SecurityTokenDescriptor
            {
                Subject= new ClaimsIdentity(claims),
                Expires= DateTime.Now.AddDays(1),
                SigningCredentials=creds

            };

            var tokenHandler= new JwtSecurityTokenHandler();

            var token= tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    
    
    
    }
}