using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTO.Input;
using ProductShop.DTO.Output;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        private static IMapper mapper;

        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //string usersJsonAsString = File.ReadAllText("../../../Datasets/users.json");
            //string productsJsonAsString = File.ReadAllText("../../../Datasets/products.json");
            //string categoriesJsonAsString = File.ReadAllText("../../../Datasets/categories.json");
            //string categoryProductsJsonAsString = File.ReadAllText("../../../Datasets/categories-products.json");

            //Console.WriteLine(ImportUsers(context, usersJsonAsString));
            //Console.WriteLine(ImportProducts(context, productsJsonAsString));
            //Console.WriteLine(ImportCategories(context, categoriesJsonAsString));
            //Console.WriteLine(ImportCategoryProducts(context, categoryProductsJsonAsString));

            Console.WriteLine(GetCategoriesByProductsCount(context));
        }

        //1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            //Making the json string to collection of Users with JsonConvert.DeserializeObject<Here is what we need to be transformed to>
            IEnumerable<UserInputDTO> users = JsonConvert.DeserializeObject<IEnumerable<UserInputDTO>>(inputJson);

            InitializeMapper();

            IEnumerable<User> mappedUsers = mapper.Map<IEnumerable<User>>(users);

            //Static mapping
            //IEnumerable<User> mappedUsers = users
            //    .Select(x => x.MapToUser())
            //    .ToList();

            context.Users.AddRange(mappedUsers);
            context.SaveChanges();

            return $"Successfully imported {mappedUsers.Count()}";
        }

        //2. Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            IEnumerable<ProductInputDTO> products = JsonConvert.DeserializeObject<IEnumerable<ProductInputDTO>>(inputJson);

            InitializeMapper();

            var mappedProducts = mapper.Map<IEnumerable<Product>>(products);

            //var mappedProducts = products
            //    .Select(x => x.MapToProduct())
            //    .ToList();

            context.Products.AddRange(mappedProducts);
            context.SaveChanges();

            return $"Successfully imported {mappedProducts.Count()}";
        }

        //3. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            IEnumerable<CategoryInputDTO> categories = JsonConvert
                .DeserializeObject<IEnumerable<CategoryInputDTO>>(inputJson)
                .Where(x => !string.IsNullOrEmpty(x.Name));

            InitializeMapper();

            var mappedCategories = mapper.Map<IEnumerable<Category>>(categories);

            context.Categories.AddRange(mappedCategories);
            context.SaveChanges();

            return $"Successfully imported {mappedCategories.Count()}"; ;
        }

        //4. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            IEnumerable<CategoryProductInputDTO> categoryProducts = JsonConvert
                .DeserializeObject<IEnumerable<CategoryProductInputDTO>>(inputJson);

            InitializeMapper();
            var mappedCategoryProducts = mapper.Map<IEnumerable<CategoryProduct>>(categoryProducts);

            context.CategoryProducts.AddRange(mappedCategoryProducts);
            context.SaveChanges();

            return $"Successfully imported {mappedCategoryProducts.Count()}";
        }




        //5. Exports Products in Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            //With Auto Mapper. Have to add CreateMap<Product, ProductOutputDTO>(); in ProductShopProfile.cs
            //And .ProjectTo<ProductOutputDTO>(mapper.ConfigurationProvider)
            //InitializeMapper();

            //var products = context
            //    .Products
            //    .Where(p => p.Price >= 500 && p.Price <= 1000)
            //    .OrderBy(p => p.Price)
            //    .ProjectTo<ProductOutputDTO>(mapper.ConfigurationProvider)
            //    .ToArray();

            //string productsToJson = JsonConvert.SerializeObject(products);

            //return productsToJson;

            //The Other Way
            var products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    ProductName = p.Name,
                    ProductPrice = p.Price.ToString("F2"),
                    SellerFullName = $"{p.Seller.FirstName} {p.Seller.LastName}"
                })
                .ToArray();

            string productsToJson = JsonConvert.SerializeObject(products);

            return productsToJson;
        }

        //6. Export Successfully Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var usersWithSoldProducts = context
                .Users
                .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(p => new
                {
                    p.FirstName,
                    p.LastName,
                    SoldProducts = p.ProductsSold.Select(ps => new
                    {
                        ProductName = ps.Name,
                        ProductPrice = ps.Price,
                        BuyerFirstName = ps.Buyer.FirstName,
                        BuyerLastName = ps.Buyer.LastName
                    })
                        .ToList()
                })
                .ToList();

            var jsonSetting = Formatting();

            var userSoldProductsToJson = JsonConvert.SerializeObject(usersWithSoldProducts, jsonSetting);

            return userSoldProductsToJson;
        }

        //7. Export Categories by Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context
                .Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .Select(c => new
                {
                    c.Name,
                    c.CategoryProducts.Count,
                    Averageprice = Math.Round(c.CategoryProducts.Average(cp => cp.Product.Price), 2),
                    TotalRevenue = Math.Round(c.CategoryProducts.Sum(cp => cp.Product.Price), 2)
                })
                .ToList();

            var jsonSettings = Formatting();

            var categoriesToJson = JsonConvert.SerializeObject(categories, jsonSettings);

            return categoriesToJson;
        }



        private static void InitializeMapper()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => { cfg.AddProfile<ProductShopProfile>(); });

            mapper = new Mapper(mapperConfiguration);
        }

        private static JsonSerializerSettings Formatting()
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonSetting = new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ContractResolver = contractResolver
            };
            return jsonSetting;
        }
    }
}

//static mapping
public static class UserMapping
{
    public static User MapToUser(this UserInputDTO userDto)
    {
        return new User()
        {
            Age = userDto.Age,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName
        };
    }
}

public static class ProductMapping
{
    public static Product MapToProduct(this ProductInputDTO productDto)
    {
        return new Product()
        {
            Name = productDto.Name,
            Price = productDto.Price,
            BuyerId = productDto.BuyerId,
            SellerId = productDto.SellerId
        };
    }
}

public static class CategoryMapping
{
    public static Category MapToCategory(this CategoryInputDTO categoryDto)
    {
        return new Category()
        {
            Name = categoryDto.Name
        };
    }
}

public static class CategoryProductMapping
{
    public static CategoryProduct MapToCategoryProduct(this CategoryProductInputDTO dto)
    {
        return new CategoryProduct()
        {
            CategoryId = dto.CategoryId,
            ProductId = dto.ProductId
        };
    }
}