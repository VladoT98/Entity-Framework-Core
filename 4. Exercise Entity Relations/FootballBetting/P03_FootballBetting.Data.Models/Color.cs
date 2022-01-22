using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P03_FootballBetting.Data.Models
{
    public class Color
    {
        public Color()
        {
            this.PrimaryKitTeam = new HashSet<Team>();
            this.SecondaryKitTeam = new HashSet<Team>();
        }

        [Key]
        public int ColorId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [InverseProperty("PrimaryKitColor")]
        public virtual ICollection<Team> PrimaryKitTeam { get; set; }

        [InverseProperty("SecondaryKitColor")]
        public virtual ICollection<Team> SecondaryKitTeam { get; set; }
    }
}
