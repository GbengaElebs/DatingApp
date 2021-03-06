using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using datingapp.api.Data;
using datingapp.api.DTO;
using datingapp.api.Helpers;
using datingapp.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace datingapp.api.Controllers
{
    [Route("users/{userId}/photos")]
    [ApiController]
    public class PhotosController: ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, 
        
        IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary= new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo= await _repo.GetPhoto(id);

            var photo =_mapper.Map<PhotoForReturndto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, 
                [FromForm]PhotoForCreationDto photoForCreationDto)
        {
                if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
                var userFromRepo =await _repo.GetUser(userId, true);

                var File= photoForCreationDto.File;

                var  uploadResult = new ImageUploadResult();

                if(File.Length > 0)
                {
                    using(var stream= File.OpenReadStream())
                    {
                        var uploadParams= new ImageUploadParams()
                        {
                            File = new FileDescription(File.Name, stream),
                            Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);

                    }
                }
                else
                {
                    return BadRequest("No Photo Uploaded");
                }
            photoForCreationDto.url =uploadResult.Uri.ToString();
            photoForCreationDto.PubliccId =uploadResult.PublicId;

            var photo= _mapper.Map<Photos>(photoForCreationDto);

            if(!userFromRepo.Photos.Any(u => u.IsMain))
            photo.IsMain = true;

            userFromRepo.Photos.Add(photo);

            if(await _repo.SaveAll())
            {
                var photoToReturn= _mapper.Map<PhotoForReturndto>(photo);
                return CreatedAtRoute("GetPhoto", new { userId= userId , id = photo.Id}, photoToReturn);
            }

            return BadRequest("could not add photo");

        }

        [HttpPost("{id}/setMain")]

        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
           if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user= await _repo.GetUser(userId, true);

                if(!user.Photos.Any(p => p.Id == id))
                return Unauthorized();
                
           var photosFromRepo= await _repo.GetPhoto(id);

           if(photosFromRepo.IsMain)
           return BadRequest("This is already the main photo");

           var currentMainPhoto =await _repo.GetMainPhotoForUser(userId);
           currentMainPhoto.IsMain =false;

           photosFromRepo.IsMain =true;

           if(await _repo.SaveAll())
           return NoContent();

           return BadRequest("could not set photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id){

            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

                var user= await _repo.GetUser(userId,true);

                if(!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

                var photoFromRepo =await _repo.GetPhoto(id);

                if(photoFromRepo.PubliccId != null)
                {
                    var deleteParems =new DeletionParams(photoFromRepo.PubliccId);

                    var result = _cloudinary.Destroy(deleteParems);

                    if(result.Result == "ok")
                    {
                    _repo.Delete(photoFromRepo);
                    }
                }

                if(photoFromRepo.PubliccId ==  null)
                {
                    _repo.Delete(photoFromRepo);

                }

                
               
                if(await _repo.SaveAll())
                    return Ok();
            return BadRequest("Failed to delete the photo");
        }
}
}