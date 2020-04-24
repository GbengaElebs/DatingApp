using System;

namespace datingapp.api.Models
{
    public class Photos
    {
        public int Id {get; set;}

        public string Url {get; set;}

        public string Description {get; set;}

        public DateTime DateAdded {get; set;}

        public bool IsMain {get; set;}

        public bool PublicId {get; set;}

        public string PubliccId {get; set;}

        public virtual User User {get; set;}

        public int UserId {get; set;}


    }
}