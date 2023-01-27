using System.Text.Json;
using Poke.Api.Database;
using Poke.Api.Models;

namespace Poke.Api.Services;

public class PokemonService : IPokemonService
{
    private readonly PokemonDbContext _dbContext;
    public PokemonService(PokemonDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    private static JsonSerializerOptions jsonSettings = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };
    
    
    public Pokemon GetRandomPokemon()
    {
        using (var httpClient = new HttpClient())
        {
            Random random = new Random();
            var pokeId = random.Next(1, 299);
            var response = httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{pokeId}").Result;
            var t = response.Content.ReadAsStringAsync().Result;
            // The following line deserializes the Json from the pokeapi into a Pokemon object we defined ourself
            var pokemon = JsonSerializer.Deserialize<Pokemon>(response.Content.ReadAsStringAsync().Result, jsonSettings);
            return pokemon;
        }
    }

    public Pokemon GetPokemonById(int id)
    {
        using (var httpClient = new HttpClient())
        {
            var response = httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{id}").Result;
            var pokemon = JsonSerializer.Deserialize<Pokemon>(response.Content.ReadAsStringAsync().Result, jsonSettings);
            
            pokemon.Id = 0;
            _dbContext.Pokemons.Add(pokemon);
            _dbContext.SaveChanges();
            return pokemon;
        }
    }

    public Pokemon GetFomDatabase(int id)
    {
        var pokemonExists = _dbContext.Pokemons.Find(id);
        if (pokemonExists != null)
        {
            return pokemonExists;
        }

        return null;
    }

    public Pokemon DeletePokemon(int id)
    {
        throw new NotImplementedException();
    }
}