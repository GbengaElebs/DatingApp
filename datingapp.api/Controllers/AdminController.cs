using System.Linq;
using System.Threading.Tasks;
using datingapp.api.Data;
using datingapp.api.DTO;
using datingapp.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace datingapp.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        public AdminController(DataContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("userswithRoles")]
        public async Task<IActionResult> GetUserwithRoles()
        {
            var userList = await _context.Users
                .OrderBy(x => x.UserName)
                .Select(user => new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Roles = (from userRole in user.UserRoles
                             join role in _context.Roles
                             on userRole.RoleId
                             equals role.Id
                             select role.Name
                    ).ToList()
                }).ToListAsync();
            return Ok(userList);
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user =  await _userManager.FindByNameAsync(userName);
            var userRoles= await _userManager.GetRolesAsync(user);
            var selectedRoles= roleEditDto.RoleNames;

            selectedRoles = selectedRoles ?? new string[] {};

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if(!result.Succeeded){
                return BadRequest("Failed to Add To roles");
            };
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
             
            if(!result.Succeeded){
                return BadRequest("Failed to remove theroles");
            };
            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratorPhotoRole")]
        [HttpGet("photosForModeration")]
        public IActionResult GetPhotosForModeration()
        {
            return Ok("Only Admins or Moderators Can See this");
        }
    }
}