using System;
using System.Collections.Generic;

namespace datingapp.api.Models
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string Gender {get; set;}

        public DateTime Created {get; set;}

        public DateTime LastLogin {get ; set;}

        public string LookingFor {get; set;}

        public string Introduction {get; set;}

        public string Interests {get; set;}

        public string City {get; set;}

        public string country{get; set;}

        public ICollection<Photos> Photos{get; set;}

    }
}