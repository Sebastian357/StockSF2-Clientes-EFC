using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockSF2_Clientes.Migrations
{
    public partial class clientes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CUIT = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CondIva = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DomicilioComercial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CondicionDeVenta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IbAlicuota = table.Column<decimal>(type: "decimal(6,4)", precision: 6, scale: 4, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoDoc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoFact = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
