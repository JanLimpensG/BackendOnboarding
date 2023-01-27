using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poke.Api.Services;
using Poke.Api.Database;
using Poke.Api.Models;
using Poke.Api.ViewModels;

namespace Poke.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PokemonController : ControllerBase
{

    private readonly IPokemonService _pokemonService;
    public PokemonController(IPokemonService pokemonService)
    {
        _pokemonService = pokemonService;
    }

    /// <Challenge>
    /// Currently this method always returns ditto instead of a random pokemon, change it so it does
    /// Done!
    /// </Challenge>
    [HttpGet("random")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public Pokemon GetRandomPokemon()
    {
        var pokemon =_pokemonService.GetRandomPokemon();
        return pokemon;
    }

    /// <Challenge>
    /// Make sure each time a pokemon is retrieved, it is added to our own database
    /// Done!
    /// </Challenge>
   // [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public Pokemon GetPokemonById(int id)
    {
        var pokemon = _pokemonService.GetPokemonById(id);
        return pokemon;
        
    }

    [Authorize]
    [HttpGet("from-database/{id}")]
    public Pokemon GetPokemonFromDabase(int id)
    {
        var pokemon = _pokemonService.GetFomDatabase(id);
        return pokemon;
    }

    [HttpGet]
    [Route("GetMoves/{id}")]
    public List<string> GetPokemonMoves(int id)
    {
        var moves = _pokemonService.GetMoves(id);
        return moves;
    }
}