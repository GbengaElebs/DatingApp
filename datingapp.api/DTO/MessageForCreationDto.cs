using System;

namespace datingapp.api.DTO
{
    public class MessageForCreationDto
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public DateTime DateSent { get; set; }

        public string Content {get; set;}

        public MessageForCreationDto()
        {
            DateSent = DateTime.Now;
        }
        
    }
}