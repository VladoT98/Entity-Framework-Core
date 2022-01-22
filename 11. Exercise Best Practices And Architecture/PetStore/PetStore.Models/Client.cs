using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http;
using PetStore.Common;

namespace PetStore.Models
{
    public class Client
    {
        public Client()
        {
            this.Id = Guid.NewGuid().ToString();
            this.PetReservations = new HashSet<PetReservation>();
            this.ProductSales = new HashSet<ProductSale>();
        }

        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(GlobalConstants.ClientNameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(GlobalConstants.ClientNameMaxLength)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(GlobalConstants.EmailMaxLength)]
        public string Email { get; set; }

        [Required]
        [MaxLength(GlobalConstants.ClientPasswordMaxLength)]
        public string Password { get; set; }

        [Required]
        [MaxLength(GlobalConstants.ClientPhoneNumberMaxLength)]
        public string PhoneNumber { get; set; }

        [ForeignKey(nameof(Address))]
        public string AddressId { get; set; }
        public virtual Address Address { get; set; }

        [ForeignKey(nameof(Card))]
        public string CardId { get; set; }
        public virtual CardInfo Card { get; set; }

        public virtual ICollection<PetReservation> PetReservations { get; set; }

        public virtual ICollection<ProductSale> ProductSales { get; set; }
    }
}
