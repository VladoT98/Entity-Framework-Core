using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetStore.Common;

namespace PetStore.Models
{
    public class Store
    {
        public Store()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Products = new HashSet<Product>();
            this.Services = new HashSet<Service>();
            this.Pets = new HashSet<Pet>();
        }

        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(GlobalConstants.StoreNameMaxLength)]
        public string Name { get; set; }

        [MaxLength(GlobalConstants.StoreDescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        [MaxLength(GlobalConstants.StoreWorkTimeMaxLength)]
        public string WorkTime { get; set; }

        [Required]
        [ForeignKey(nameof(Address))]
        public string AddressId { get; set; }
        public virtual Address Address { get; set; }

        [Required]
        [MaxLength(GlobalConstants.StoreEmailMaxLength)]
        public string Email { get; set; }

        [Required]
        [MaxLength(GlobalConstants.StorePhoneNumberMaxLength)]
        public string PhoneNumber { get; set; }

        public virtual ICollection<Pet> Pets { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<Service> Services { get; set; }
    }
}
