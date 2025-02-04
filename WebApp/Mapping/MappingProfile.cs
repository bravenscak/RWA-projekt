using AutoMapper;
using MiniOglasnikZaBesplatneStvariLibrary.Models;
using MiniOglasnikZaBesplatneStvariMvc.Models;

namespace MiniOglasnikZaBesplatneStvariMvc.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Item, ItemViewModel>();
            CreateMap<ItemType, ItemTypeViewModel>();
            CreateMap<UserDetail, UserDetailViewModel>();
            CreateMap<Tag, TagViewModel>();
        }
    }
}
