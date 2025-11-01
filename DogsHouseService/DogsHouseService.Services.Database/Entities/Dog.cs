using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogsHouseService.Services.Database.Entities
{
    /// <summary>
    /// Dog entity
    /// </summary>
    public class Dog
    {
        /// <summary>
        /// Gets or sets the dog name (Primary Key).
        /// </summary>
        [Key]
        [Column("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the dog color
        /// </summary>
        [Column("color")]
        public string Color { get; set; } = null!;

        /// <summary>
        /// Gets or sets the dog tail length.
        /// </summary>
        [Column("tail_length")]
        public int TailLength { get; set; }

        /// <summary>
        /// Gets or sets the dog weight.
        /// </summary>
        [Column("weight")]
        public int Weight { get; set; }
    }
}
