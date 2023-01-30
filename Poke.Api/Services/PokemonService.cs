using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                var moveName = moveTemp.Moves[index].Move.Name;
                moves.Add(moveName);
                var moveExists = _dbContext.Moves.Where(m => m.Name == moveName).ToList();
                if (!moveExists.Any())
                {
                    var newMove = new Move()
                    {
                        Name = moveName
                    };
                    _dbContext.Moves.Add(newMove);
                    _dbContext.SaveChanges();
                }

            }
            //Check for the ids of the added moves to add to the Many to Many table
            var manyToManyMoves = new List<Move>();
            for (int i = 0; i < 4; i++)
            {
                manyToManyMoves.Add(_dbContext.Moves.Where(x => x.Name == moves[i]).ToList().First());
            }

            pokemon.Moves = manyToManyMoves;
            //Add the pokemon to the database
            pokemon.Id = 0;
            _dbContext.Pokemons.Add(pokemon);
            _dbContext.SaveChanges();
            
            return pokemon;
        }
    }

    public Pokemon GetFomDatabase(int id)
    {
        var pokemonExists = _dbContext.Pokemons.Include(p => p.Moves).Where(p => p.Id == id).First();
        if (pokemonExists != null)
        {
            
            return pokemonExists;
        }

        return null;
    }

    public Pokemon DeletePokemon(int id)
    {
        var pokemonExists = _dbContext.Pokemons.Find(id);
        _dbContext.Pokemons.Remove(pokemonExists);
        _dbContext.SaveChanges();

        return pokemonExists;
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
                var moveName = moveTemp.Moves[index].Move.Name;
                moves.Add(moveName);
                var moveExists = _dbContext.Moves.Where(m => m.Name == moveName).ToList();
                if (!moveExists.Any())
                {
                    var newMove = new Move()
                    {
                        Name = moveName
                    };
                    _dbContext.Moves.Add(newMove);
                    _dbContext.SaveChanges();
                }

            }
            moves.Sort();
            return moves;
        }
    }
}