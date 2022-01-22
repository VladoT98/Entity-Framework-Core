using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetStore.Common;

namespace PetStore.Models
{
    public class Product
    {
        public Product()
        {
            this.Id = Guid.NewGuid().ToString();
            this.AvailableStores = new HashSet<Store>();
            this.Sales = new HashSet<ProductSale>();
        }

        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(GlobalConstants.ProductNameMaxLength)]
        public string Name { get; set; }

        [MaxLength(GlobalConstants.ProductDescriptionMaxLength)]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        [Required]
        [MaxLength(GlobalConstants.ProductDistributorMaxLength)]
        public string DistributorName { get; set; }

        public bool InStock => this.Quantity > 0;

        [ForeignKey(nameof(ProductType))]
        public int ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }

        public virtual ICollection<Store> AvailableStores { get; set; }

        public virtual ICollection<ProductSale> Sales { get; set; }
    }
}
