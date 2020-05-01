using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using datingapp.api.Data;
using datingapp.api.DTO;
using datingapp.api.Helpers;
using datingapp.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace datingapp.api.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
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
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo= await _repo.GetUser(currentUserId,true);
            if(string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }
            userParams.UserId= currentUserId;
            
            var users = await _repo.GetUsers(userParams);
            var userToReturn = _mapper.Map<IEnumerable<UserForList>>(users);
        
            Response.AddPagination(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages);
            return Ok(userToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var iscurrentUserId= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;

            var users = await _repo.GetUser(id,iscurrentUserId);
            var userToReturn = _mapper.Map<UserForDetaileddto>(users);


            return Ok(userToReturn);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
                var userFromRepo =await _repo.GetUser(id, true);

                _mapper.Map(userForUpdateDto , userFromRepo);

                if(await _repo.SaveAll())
                {
                    return NoContent();
                }
                throw new Exception($"Updating user {id} failed on save");
        }

        [HttpPost("{id}/like/{recipientId}")]

        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
             if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            if(id == recipientId)
            {
                return  BadRequest("Not permitted");
            }

            var like = await _repo.GetLike(id, recipientId);

            if(like !=null)
            {
                return BadRequest("You already like this user");
            }

            if(await _repo.GetUser(recipientId, true) == null)
            {
                return NotFound();
            }

            like = new Like
            {
                LikerId =id,
                LikeeId = recipientId
            };

            _repo.Add<Like>(like);

            if(await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to like a user");
        }
    }
}
