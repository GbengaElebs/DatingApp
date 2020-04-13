using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using datingapp.api.Data;
using datingapp.api.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace datingapp.api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            
            _mapper = mapper;
            _repo = repo;

        }
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            var userToReturn = _mapper.Map<IEnumerable<UserForList>>(users);

            return Ok(userToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var users = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetaileddto>(users);


            return Ok(userToReturn);
        }
    }
}
