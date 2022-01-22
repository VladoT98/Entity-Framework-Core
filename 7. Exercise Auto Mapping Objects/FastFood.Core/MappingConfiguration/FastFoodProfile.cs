using System.Collections.Generic;
using FastFood.Core.ViewModels.Categories;
using FastFood.Services.DTO.Category;

namespace FastFood.Core.MappingConfiguration
{
    using AutoMapper;
    using FastFood.Models;
    using ViewModels.Positions;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            //Positions
            this.CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            this.CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            //Categories
            this.CreateMap<CreateCategoryInputModel, CreateCategoryDTO>();

            this.CreateMap<ListAllCategoriesDTO, CategoryAllViewModel>()
                .ForMember(x => x.Name,
                    y => y.MapFrom(s => s.CategoryName));

            this.CreateMap<CreateCategoryDTO, Category>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.CategoryName));

            this.CreateMap<Category, ListAllCategoriesDTO>()
                .ForMember(x => x.CategoryName,
                    y => y.MapFrom(s => s.Name));
            //this.CreateMap<CategoryAllViewModel, Employee>();
        }
    }
}
