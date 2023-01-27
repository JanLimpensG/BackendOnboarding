namespace Poke.Api.ViewModels;

public class PokemonMovesVM
{
    public List<MovesVM> Moves { get; set; }
}

public class MovesVM
{
    public Moves2VM Move { get; set; }
}

public class Moves2VM
{
    public string Name { get; set; }
}