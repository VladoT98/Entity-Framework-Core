using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetStore.Common;

namespace PetStore.Models
{
    public class CardInfo
    {
        public CardInfo()
        {
            this.Id = Guid.NewGuid().ToString();
            this.ProductSales = new HashSet<ProductSale>();
        }

        [Key]
        public string Id { get; set; }

        [Required]
        [ForeignKey(nameof(Owner))]
        public string ClientId { get; set; }
        public virtual Client Owner { get; set; }

        [Required]
        [MaxLength(GlobalConstants.CardNumberMaxLength)]
        public string CardNumber { get; set; }

        [Required]
        [MaxLength(GlobalConstants.CardHolderMaxLength)]
        public string CardHolder { get; set; }

        [Required]
        [MaxLength(GlobalConstants.CardExpirationDateMaxLength)]
        public string ExpirationDate { get; set; }

        [Required]
        [MaxLength(GlobalConstants.CVCMaxLength)]
        public string CVC { get; set; }

        public virtual ICollection<ProductSale> ProductSales { get; set; }
    }
}
