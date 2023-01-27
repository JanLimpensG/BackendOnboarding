using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Poke.Api.Models;

namespace Poke.Api.Database;

public class PokemonDbContext : IdentityDbContext<IdentityUser>
{
    public PokemonDbContext(DbContextOptions<PokemonDbContext> options) : base(options)
    {
    }
    

    public DbSet<Pokemon> Pokemons { get; set; }
    public DbSet<Move> Moves { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
