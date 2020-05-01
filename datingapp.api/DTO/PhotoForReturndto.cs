using System;
using Microsoft.AspNetCore.Http;

namespace datingapp.api.DTO
{
    public class PhotoForReturndto
    {
        public string url {get; set;}
        public int Id {get; set;}
        public string Description {get; set;}
        public DateTime DateAdded {get; set;}
        public string PubliccId {get; set;}
        public bool IsMain {get; set;}
        public bool IsApproved {get; set;}

    }
}