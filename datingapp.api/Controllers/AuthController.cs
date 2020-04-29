using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using datingapp.api.Data;
using datingapp.api.DTO;
using datingapp.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace datingapp.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config,
        IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _config = config;
            _repo = repo;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserForRegister userforRegister)
        {
            // if (!ModelState.IsValid) return BadRequest(ModelState);

            // userforRegister.UserName = userforRegister.UserName.ToLower();

            // if (await _repo.UserExists(userforRegister.UserName))
            //     return BadRequest("UserName already Exists");

            // var UserToCreate = new User
            // {
            //     UserName = userforRegister.UserName
            // };
            var UserToCreate = _mapper.Map<User>(userforRegister);
            var result = await _userManager.CreateAsync(UserToCreate, 
            userforRegister.Password);

            // var createdUser = await _repo.Register(UserToCreate, userforRegister.Password);

            var usertoreturn = _mapper.Map<UserForDetaileddto>(UserToCreate);
            if(result.Succeeded){
            return CreatedAtRoute("GetUser", new { controller = "Users", 
            id = UserToCreate.Id }, usertoreturn);

            }
            
            return BadRequest(result.Errors);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserForLogin userForLogin)
        {

            var user = await _userManager.FindByNameAsync(userForLogin.userName);

            var result= await _signInManager.CheckPasswordSignInAsync(user, userForLogin.password,false);
            if (result.Succeeded)
            {
                var userForReturn = _mapper.Map<UserForList>(user);
                return Ok(new
            {
                token = GenerateJwtToken(user).Result,
                user= userForReturn
            });

            }
                return Unauthorized();        
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier ,user.Id.ToString()),
                new Claim(ClaimTypes.Name ,user.UserName)


            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

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
            return tokenHandler.WriteToken(token);

        }

    }

}