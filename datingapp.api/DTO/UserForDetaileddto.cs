using System;
using System.Collections.Generic;
using datingapp.api.Models;

namespace datingapp.api.DTO
{
    public class UserForDetaileddto
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public int Age { get; set; }

        public string KnownAs { get; set; }

        public string Gender {get; set;}

        public DateTime Created {get; set;}

        public DateTime LastActive {get ; set;}

        public string City {get; set;}

        public string Country{get; set;}

        public string PhotosUrl{get; set;}

        public string LookingFor {get; set;}


        public ICollection<PhotosForDetaileddto> Photos {get; set;}

    }
}