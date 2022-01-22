using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PetStore.Common;

namespace PetStore.Models
{
    public class Address
    {
        public Address()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Stores = new HashSet<Store>();
            this.Clients = new HashSet<Client>();
            this.ProductSales = new HashSet<ProductSale>();
        }

        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(GlobalConstants.AddressTownMaxLength)]
        public string TownName { get; set; }

        [Required]
        [MaxLength(GlobalConstants.AddressTextMaxLength)]
        public string Text { get; set; }

        [MaxLength(GlobalConstants.AddressNotesMaxLength)]
        public string Notes { get; set; }

        public virtual ICollection<Store> Stores { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public virtual ICollection<ProductSale> ProductSales { get; set; }
    }
}
