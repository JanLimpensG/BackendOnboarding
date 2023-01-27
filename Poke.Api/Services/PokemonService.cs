using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Poke.Api.Database;
using Poke.Api.Models;
using Poke.Api.ViewModels;

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
            //Get the pokemon form that id
            var pokemon = JsonSerializer.Deserialize<Pokemon>(response.Content.ReadAsStringAsync().Result, jsonSettings);
            //Get the moves the pokemon can learn
            var moveTemp = JsonSerializer.Deserialize<PokemonMovesVM>(response.Content.ReadAsStringAsync().Result, jsonSettings);
            //Store 4 random possible moves into string ,and into db
            var moves = new List<string>();
            Random random = new Random();
            for (int i = 0; i < 4; i++)
            {
                var index = random.Next(0, moveTemp.Moves.Count);
                moves.Add(moveTemp.Moves[index].Move.Name);
                var newMove = new Move()
                {
                    Name = moveTemp.Moves[index].Move.Name
                };
                // TODO: Check if the move doesnot exist, and if it doesn't add it
                
            }
            //TODO: Check for the ids of the added moves to add to the Many to Many table
            
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

    public List<string> GetMoves(int id)
    {
        using (var httpClient = new HttpClient())
        {
            var response = httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{id}").Result;
            var moveTemp = JsonSerializer.Deserialize<PokemonMovesVM>(response.Content.ReadAsStringAsync().Result, jsonSettings);

            var moves = new List<string>();
            Random random = new Random();
            for (int i = 0; i < 4; i++)
            {
                var index = random.Next(0, moveTemp.Moves.Count);
                moves.Add(moveTemp.Moves[index].Move.Name);
            }

            return moves;
        }
    }
}