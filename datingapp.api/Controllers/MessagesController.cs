
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
    [Authorize]
    [Route("{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;

        }

        [HttpGet("{id}", Name="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId,int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var messageFromRepo = await _repo.GetMessage(id);

            if(messageFromRepo == null)
                return NotFound();

            return Ok(messageFromRepo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage (int userId, MessageForCreationDto messageForCreationDto)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;

            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);

            if(recipient == null)
                return BadRequest("could not find user");

            var message= _mapper.Map<Message>(messageForCreationDto);
            _repo.Add(message);
             
            var messagetoreturn= _mapper.Map<MessageForCreationDto>(message);


            if(await _repo.SaveAll())
            {
                    return CreatedAtRoute("GetMessage", new {userId , id = message.Id}, messagetoreturn);
            }

            throw new Exception("failed to save message");
            
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageParams.UserId = userId;

            var messageFromRepo = await _repo.GetMessagesForUser(messageParams);

            var messages= _mapper.Map<IEnumerable<Messagetoreturndto>>(messageFromRepo);

            Response.AddPagination(messageFromRepo.CurrentPage,messageFromRepo.PageSize,messageFromRepo.TotalCount,
            messageFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messagesFromrepoThread = await _repo.GetMessageThread(userId, recipientId);

            var messageThread= _mapper.Map<IEnumerable<Messagetoreturndto>>(messagesFromrepoThread);

            return Ok(messageThread);
        }
    
    }
}