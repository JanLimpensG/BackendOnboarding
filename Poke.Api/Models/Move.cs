using System.Text.Json.Serialization;

namespace Poke.Api.Models;

public class Move
{
    public int ID { get; set; }
    public string Name { get; set; }
    
    //Many-to-many relation with pokemon
    [JsonIgnore]
    public virtual ICollection<Pokemon> Pokemons { get; set; }

    public Move()
    {
        this.Pokemons = new HashSet<Pokemon>();
    }
}