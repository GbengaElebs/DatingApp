using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using datingapp.api.Data;
using datingapp.api.DTO;
using datingapp.api.Helpers;
using datingapp.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace datingapp.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;

        private Cloudinary _cloudinary;
        public AdminController(DataContext context, UserManager<User> userManager,
        IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;
            _userManager = userManager;
            _context = context;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
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
            var user = await _userManager.FindByNameAsync(userName);
            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = roleEditDto.RoleNames;

            selectedRoles = selectedRoles ?? new string[] { };

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to Add To roles");
            };
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to remove theroles");
            };
            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratorPhotoRole")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            var photosForApproval = await _repo.GetPhotosforApproval();
                var photo= await _context.photos.IgnoreQueryFilters().
                                        Include(u => u.User)
                                        .Where(p => p.IsApproved == false)
                                        .Select(u => new {
                                            Id = u.Id,
                                            UserName= u.User.UserName,
                                            Url= u.Url,
                                            IsApproved= u.IsApproved,
                                            PubliccId=u.PubliccId
                                        }).ToListAsync();
            return Ok(photo);
        }

        [Authorize(Policy = "ModeratorPhotoRole")]
        [HttpPost("ApprovePhotosForUser/{PubliccId}")]
        public async Task<IActionResult> ApprovePhotosForUser(string PubliccId)
        {
            var userPhotoForApproval = await _repo.GetPhotoforApproval(PubliccId);
            userPhotoForApproval.IsApproved = true;

            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to approve Photo");
        }

        [Authorize(Policy = "ModeratorPhotoRole")]
        [HttpDelete("DisapprovePhotoForUser/{PubliccId}")]
        public async Task<IActionResult> DisapprovePhotoForUser( string PubliccId)
        {
            var userPhotoForApproval = await _repo.GetPhotoforApproval(PubliccId);
            
            if (userPhotoForApproval.PubliccId != null)
            {
                var deleteParems = new DeletionParams(userPhotoForApproval.PubliccId);

                var result = _cloudinary.Destroy(deleteParems);

                if (result.Result == "ok")
                {
                    _repo.Delete(userPhotoForApproval);
                }
            }

            if (userPhotoForApproval.PubliccId == null)
            {
                _repo.Delete(userPhotoForApproval);

            }

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest("Failed to delete the photo");

        }
    }
}