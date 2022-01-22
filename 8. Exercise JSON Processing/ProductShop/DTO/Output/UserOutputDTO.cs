using System.Collections.Generic;
using ProductShop.Models;

namespace ProductShop.DTO.Output
{
    public class UserOutputDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IEnumerable<Product> SoldProducts { get; set; }
    }
}
