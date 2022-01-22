using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PetStore.Common;

namespace PetStore.Models
{
    public class PetType
    {
        public PetType()
        {
            this.Pets = new HashSet<Pet>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(GlobalConstants.PetTypeNameMaxLength)]
        public string TypeName { get; set; }

        public virtual ICollection<Pet> Pets { get; set; }
    }
}
