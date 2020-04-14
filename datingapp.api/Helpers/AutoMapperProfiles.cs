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
                ).ForMember( dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));;
            CreateMap<Photos , PhotosForDetaileddto>();
            CreateMap<UserForUpdateDto , User>();

        }
        
    }
}