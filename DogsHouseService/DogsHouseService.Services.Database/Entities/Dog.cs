using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogsHouseService.Services.Database.Entities
{
    public class Dog
    {
        [Key]
        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("color")]
        public string Color { get; set; } = null!;

        [Column("tail_length")]
        public int TailLength { get; set; }

        [Column("weight")]
        public int Weight { get; set; }
    }
}
