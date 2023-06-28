using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SudokuBackend {
    public class MappingProfile : Profile {
        public MappingProfile() {
            CreateMap<UserForRegistrationDto, User>();
            CreateMap<GameDto, Game>();
            CreateMap<GameDto, Game>().ReverseMap();

            CreateMap<UserGameDto, UserGame>();
            CreateMap<UserGameDto, UserGame>().ReverseMap();

            CreateMap<Highscore, HighscoreDto>();
        }
    }
}
