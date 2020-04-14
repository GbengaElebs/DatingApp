using System;
using Microsoft.AspNetCore.Http;

namespace datingapp.api.DTO
{
    public class PhotoForCreationDto
    {
        public string url {get; set;}
        public IFormFile File {get; set;}
        public string Description {get; set;}
        public DateTime DateAdded {get; set;}
        public string PubliccId {get; set;}

        public PhotoForCreationDto()
        {
            DateAdded =DateTime.Now;
        }


    }
}