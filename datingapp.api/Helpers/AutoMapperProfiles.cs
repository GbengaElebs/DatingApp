using System.Linq;
using AutoMapper;
using datingapp.api.DTO;
using datingapp.api.Models;

namespace datingapp.api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForList>()
                .ForMember(dest => dest.PhotosUrl, opt =>
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url)
                ).ForMember( dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<User , UserForDetaileddto>()
                 .ForMember(dest => dest.PhotosUrl, opt =>
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url)
                ).ForMember( dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photos , PhotosForDetaileddto>();
            CreateMap<UserForUpdateDto , User>();
            CreateMap<Photos , PhotoForReturndto>();
            CreateMap<PhotoForCreationDto , Photos>();
            CreateMap<UserForRegister , User>();
            CreateMap<User , UserForRegister>();
            CreateMap<MessageForCreationDto , Message>().ReverseMap();
            CreateMap<Message , Messagetoreturndto>()
                .ForMember(m => m.SenderPhotoUrl, opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(m => m.RecipientPhotoUrl, opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));






        }
        
    }
}