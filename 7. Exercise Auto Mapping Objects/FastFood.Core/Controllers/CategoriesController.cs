using System.Collections.Generic;
using System.Linq;
using FastFood.Services.DTO.Category;
using FastFood.Services.Interfaces;

namespace FastFood.Core.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Categories;

    public class CategoriesController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;

        public CategoriesController(IMapper mapper, ICategoryService categoryService)
        {
            this.categoryService = categoryService;
            this.mapper = mapper;
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Create(CreateCategoryInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.RedirectToAction("Create");
            }

            CreateCategoryDTO categoryDto = this.mapper.Map<CreateCategoryDTO>(model);

            this.categoryService.Create(categoryDto);

            return this.RedirectToAction("All");
        }

        public IActionResult All()
        {
            ICollection<ListAllCategoriesDTO> categoriesDto = this.categoryService.All();

            List<CategoryAllViewModel> categoryViewModels =
                this.mapper
                    .Map<ICollection<ListAllCategoriesDTO>,
                        ICollection<CategoryAllViewModel>>(categoriesDto)
                    .ToList();

            return this.View("All", categoryViewModels);
        }
    }
}
