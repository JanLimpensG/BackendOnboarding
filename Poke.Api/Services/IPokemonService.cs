using Poke.Api.Models;

namespace Poke.Api.Services;

public interface IPokemonService
{
    public Pokemon GetRandomPokemon();
    public Pokemon GetPokemonById(int id);
    public Pokemon GetFomDatabase(int id);
    public Pokemon DeletePokemon(int id);
}