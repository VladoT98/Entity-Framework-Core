using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PetStore.Common;
using PetStore.Models.Enum;

namespace PetStore.Models
{
    public class Pet
    {
        public Pet()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(GlobalConstants.PetNameMaxLength)]
        public string Name { get; set; }

        public int Age { get; set; }

        [MaxLength(GlobalConstants.PetDescriptionMaxLength)]
        public string Description { get; set; }

        [ForeignKey(nameof(Breed))]
        public string BreedId { get; set; }
        public virtual Breed Breed { get; set; }

        public GenderEnum Gender { get; set; }

        public decimal Price { get; set; }

        public bool isSold { get; set; }

        [Required]
        [ForeignKey(nameof(Store))]
        public string StoreId { get; set; }
        public virtual Store Store { get; set; }

        [ForeignKey(nameof(Reservation))]
        public string ReservationId { get; set; }
        public virtual PetReservation Reservation { get; set; }

        [ForeignKey(nameof(PetType))]
        public int PetTypeId { get; set; }
        public virtual PetType PetType { get; set; }
    }
}