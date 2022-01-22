using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FastFood.Data;
using FastFood.Models;
using FastFood.Services.DTO.Category;
using FastFood.Services.Interfaces;

namespace FastFood.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly FastFoodContext dbContext;
        private readonly IMapper mapper;

        public CategoryService(FastFoodContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public void Create(CreateCategoryDTO dto)
        {
            Category category = this.mapper.Map<Category>(dto);

            this.dbContext.Categories.Add(category);
            this.dbContext.SaveChanges();
        }

        public ICollection<ListAllCategoriesDTO> All() => this.dbContext
            .Categories
            .ProjectTo<ListAllCategoriesDTO>
                (this.mapper.ConfigurationProvider)
            .ToList();
    }
}
