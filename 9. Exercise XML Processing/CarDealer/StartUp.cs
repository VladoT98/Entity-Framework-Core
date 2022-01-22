using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using CarDealer.Data;
using CarDealer.DTO.Import;
using CarDealer.DTO.Input;
using CarDealer.Models;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();

            //DbReset(context);

            string inputXml = File.ReadAllText("../../../Datasets/cars.xml");
            string result = ImportCars(context, inputXml);

            Console.WriteLine(result);
        }

        //9. import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            //FIRST STEP. Here we create root and the ability to serialize and deserialize via XmlSerializer(just the name of the class, it does not serialize the object!) the xml from the Suppliers.xml
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Suppliers");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSupplierDto[]), xmlRoot);
            //---------------------------------------------------------------------------------------

            //SECOND STEP. Allows us to read the inputXml chunk by chunk, it is a stream
            using StringReader stringReader = new StringReader(inputXml);
            //---------------------------------------------------------------------------------------

            //THIRD STEP. Here we deserialize(It is the process of getting back the serialized object so that it can be loaded into memory.) the xmlSerializer
            ImportSupplierDto[] supplierDtos = (ImportSupplierDto[])xmlSerializer.Deserialize(stringReader);
            //---------------------------------------------------------------------------------------

            //FORTH STEP. Adding suppliers to context.Suppliers
            ICollection<Supplier> suppliers = new HashSet<Supplier>();
            foreach (var supplierDto in supplierDtos)
            {
                Supplier supplier = new Supplier()
                {
                    Name = supplierDto.Name,
                    IsImporter = bool.Parse(supplierDto.IsImporter)
                };

                suppliers.Add(supplier);
            }
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
            //---------------------------------------------------------------------------------------

            return $"Successfully imported {suppliers.Count}";
        }

        //10. import parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = XmlSerializer("Parts", typeof(importPartDto[]));

            using StringReader stringReader = new StringReader(inputXml);

            importPartDto[] partDtos = (importPartDto[])xmlSerializer.Deserialize(stringReader);

            ICollection<Part> parts = new HashSet<Part>();
            foreach (var partDto in partDtos)
            {
                Supplier supplier = context
                    .Suppliers
                    .Find(partDto.SupplierId);

                if (supplier != null)
                {
                    Part part = new Part()
                    {
                        Name = partDto.Name,
                        Price = partDto.Price,
                        Quantity = partDto.Quantity,
                        SupplierId = partDto.SupplierId
                    };

                    parts.Add(part);
                }
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        //11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = XmlSerializer("Cars", typeof(ImportCarDto[]));

            using StringReader stringReader = new StringReader(inputXml);
            ImportCarDto[] carDtos = (ImportCarDto[])xmlSerializer.Deserialize(stringReader);

            ICollection<Car> cars = new HashSet<Car>();

            foreach (var carDto in carDtos)
            {
                Car car = new Car()
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TraveledDistance,
                };

                ICollection<PartCar> carParts = new HashSet<PartCar>();
                foreach (var partId in carDto.Parts.Select(p => p.Id).Distinct())
                {
                    Part part = context
                        .Parts
                        .Find(partId);

                    if (part != null)
                    {
                        PartCar carPart = new PartCar()
                        {
                            PartId = partId
                        };

                        carParts.Add(carPart);
                    }
                }

                car.PartCars = carParts;
                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        //12. Import Customers
        //public static string ImportCustomers(CarDealerContext context, string inputXml)
        //{
        //    XmlRootAttribute xmlRoot = new XmlRootAttribute("Customers");
        //    var xmlSerializer = XmlSerializer("Customer", typeof(ImportCustomerDto[]));

        //    using StringReader stringReader = new StringReader(inputXml);
        //    ImportCustomerDto[] customerDtos = (ImportCustomerDto[])xmlSerializer.Deserialize(stringReader);

        //    ICollection<Customer> customers = new HashSet<Customer>();
        //    foreach (var customerDto in customerDtos)
        //    {
        //        Customer customer = new Customer()
        //        {
        //            Name = customerDto.Name,
        //            BirthDate = DateTime.Parse(customerDto.BirthDate),
        //            IsYoungDriver = 
        //        }
        //    }

        //    return "";
        //}

        private static XmlSerializer XmlSerializer(string rootName, Type dtoType)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(dtoType, xmlRoot);
            return xmlSerializer;
        }

        private static void DbReset(CarDealerContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            Console.WriteLine("DB reset was successful!");
        }
    }
}