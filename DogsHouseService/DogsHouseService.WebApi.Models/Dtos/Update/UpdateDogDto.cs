using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DogsHouseService.WebApi.Models.Dtos.Update
{
    /// <summary>
    /// DTO for updating a dog
    /// </summary>
    public class UpdateDogDto
    {
        /// <summary>
        /// Gets or sets the name of the dog.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the color of the dog.
        /// </summary>
        [JsonPropertyName("color")]
        public string Color { get; set; } = null!;

        /// <summary>
        /// Gets or sets the tail length of the dog.
        /// </summary>
        [JsonPropertyName("tail_length")]
        public int TailLength { get; set; }

        /// <summary>
        /// Gets or sets the weight of the dog.
        /// </summary>
        [JsonPropertyName("weight")]
        public int Weight { get; set; }
    }
}
