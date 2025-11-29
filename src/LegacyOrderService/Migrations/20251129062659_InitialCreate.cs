using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegacyOrderService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename existing Orders table to preserve legacy data
            migrationBuilder.Sql("ALTER TABLE Orders RENAME TO Orders_Legacy_Backup;");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),

                    CustomerName = table.Column<string>(type: "TEXT", nullable: false),
                    ProductName = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<long>(type: "INTEGER", nullable: false),
                    Price = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.Sql(@"
                INSERT INTO Orders (Id, CustomerName, ProductName, Quantity, Price)
                SELECT 
                    Id,                            
                    COALESCE(CustomerName, 'Unknown'), 
                    COALESCE(ProductName, 'Unknown'), 
                    COALESCE(Quantity, 0),
                    COALESCE(Price, 0)
                FROM Orders_Legacy_Backup;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Orders");
            migrationBuilder.DropTable(name: "Products");
         
            migrationBuilder.Sql("ALTER TABLE Orders_Legacy_Backup RENAME TO Orders;");
        }
    }
}
