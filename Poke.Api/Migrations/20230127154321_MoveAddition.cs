using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poke.Api.Migrations
{
    public partial class MoveAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Moves",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moves", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MovePokemon",
                columns: table => new
                {
                    MovesID = table.Column<int>(type: "int", nullable: false),
                    PokemonsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovePokemon", x => new { x.MovesID, x.PokemonsId });
                    table.ForeignKey(
                        name: "FK_MovePokemon_Moves_MovesID",
                        column: x => x.MovesID,
                        principalTable: "Moves",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovePokemon_Pokemons_PokemonsId",
                        column: x => x.PokemonsId,
                        principalTable: "Pokemons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovePokemon_PokemonsId",
                table: "MovePokemon",
                column: "PokemonsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovePokemon");

            migrationBuilder.DropTable(
                name: "Moves");
        }
    }
}
