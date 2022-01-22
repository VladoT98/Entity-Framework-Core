using System.Collections.Generic;
using FastFood.Services.DTO.Category;

namespace FastFood.Services.Interfaces
{
    public interface ICategoryService
    {
        void Create(CreateCategoryDTO dto);

        ICollection<ListAllCategoriesDTO> All();
    }
}
