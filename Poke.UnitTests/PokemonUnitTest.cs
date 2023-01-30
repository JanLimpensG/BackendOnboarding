using Microsoft.Extensions.Hosting.Internal;
using Poke.Api.Controllers;
using Poke.Api.Services;
using Poke.Api.Models;
using Moq;
using Xunit.Abstractions;

namespace Poke.UnitTests;

public class PokemonUnitTest
{
    private readonly Mock<IPokemonService> _pokemonService;
    //This is used to output usefull messages instead of console lines
    private readonly ITestOutputHelper _testOutputHelper;

    public PokemonUnitTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _pokemonService = new Mock<IPokemonService>();
    }
    
    /// <Challenge>
    /// Currently this test only checks if it is not null, make sure it tests it returns a different pokemon each time
    /// </Challenge>
    [Fact]
    public void GetRandomPokemonReturnsPokemon()
    {
        //arrange
        Random random = new Random();
        var pokemonList = GetMockPokemon();
        Random random2 = new Random();
        var controller = new PokemonController(_pokemonService.Object);
        _pokemonService.Setup(x => x.GetRandomPokemon())
            .Returns(pokemonList[random.Next(0, 12)]);
        var result1 = controller.GetRandomPokemon();
        _pokemonService.Setup(x => x.GetRandomPokemon())
            .Returns(pokemonList[random2.Next(0, 12)]);
        var result2 = controller.GetRandomPokemon();
        
        //assert
        Assert.NotNull(result1.Value);
        Assert.NotNull(result2.Value);
        
        Assert.False(result1.Value.Name.Equals(result2.Value.Name));
        //Assert.False(result1.Id.Equals(result2.Id));
        
        Assert.NotSame(result1,result2);
    }

    [Fact]
    public void GetPokemonFromId()
    {
        //arrange
        var pokemonList = GetMockPokemon();
        var controller = new PokemonController(_pokemonService.Object);
        _pokemonService.Setup(x => x.GetPokemonById(4))
            .Returns(pokemonList[4]);
        //act
        var pokemon = controller.GetPokemonById(4);
        //assert
        
        Assert.NotNull(pokemon);
        Assert.Same(pokemon, pokemonList[4]);
    }

    [Fact]
    public void GetPokemonFromDatabase()
    {
        var pokemonList = GetMockPokemon();
        var controller = new PokemonController(_pokemonService.Object);
        _pokemonService.Setup(x => x.GetFomDatabase(11))
            .Returns(pokemonList[11]);
        
        var pokemon = controller.GetPokemonFromDatabase(11);
        
        //assert
        Assert.NotNull(pokemon);
        Assert.Same(pokemon, pokemonList[11]);
    }

    [Fact]
    public void DeletePokemonFromDatabase()
    {
        //arrange
        var pokemonList = GetMockPokemon();
        var controller = new PokemonController(_pokemonService.Object);
        _pokemonService.Setup(x => x.DeletePokemon(5))
            .Returns(pokemonList[4]);
        //act
        var pokemon = controller.DeletePokemon(5);
        //assert
        Assert.NotNull(pokemon);
        Assert.Same(pokemon, pokemonList[4]);
    }


    private List<Pokemon> GetMockPokemon()
    {
        List<Pokemon> pokemonData = new List<Pokemon>
        {
            new Pokemon
            {
               Id = 1,
               Name = "bulbasaur",
               Weight = 69,
               Height = 7
            },
            new Pokemon
            {
                Id = 2,
                Name = "ivysaur",
                Weight = 130,
                Height = 10
            },
            new Pokemon
            {
                Id = 3,
                Name = "venosaur",
                Weight = 1000,
                Height = 20
            },
            new Pokemon
            {
                Id = 4,
                Name = "charmander",
                Weight = 85,
                Height = 6
            },
            new Pokemon
            {
                Id = 5,
                Name = "charmeleon",
                Weight = 190,
                Height = 11
            },
            new Pokemon
            {
                Id = 6,
                Name = "charizard",
                Weight = 905,
                Height = 17
            },
            new Pokemon
            {
                Id = 7,
                Name = "squirtle",
                Weight = 905,
                Height = 17
            },
            new Pokemon
            {
                Id = 8,
                Name = "squortle",
                Weight = 905,
                Height = 17
            },
            new Pokemon
            {
                Id = 9,
                Name = "blastoise",
                Weight = 905,
                Height = 17
            },
            new Pokemon
            {
                Id = 10,
                Name = "caterpie",
                Weight = 905,
                Height = 17
            },
            new Pokemon
            {
                Id = 11,
                Name = "metapod",
                Weight = 905,
                Height = 17
            },
            new Pokemon
            {
                Id = 12,
                Name = "butterfree",
                Weight = 905,
                Height = 17
            },
        };
        return pokemonData;
    }
    
}
