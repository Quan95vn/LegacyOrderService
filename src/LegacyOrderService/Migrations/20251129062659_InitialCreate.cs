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
            var backupTableName = $"Orders_Backup_{DateTime.Now:yyyyMMdd_HHmmss}";

            // --- SAFELY RENAME LEGACY TABLE ---
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS Orders (
                    Id INTEGER PRIMARY KEY, 
                    CustomerName TEXT, 
                    ProductName TEXT, 
                    Quantity INTEGER, 
                    Price REAL
                );
            ");
            migrationBuilder.Sql($"ALTER TABLE Orders RENAME TO {backupTableName};");

            // Create Products table
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Products", x => x.Id));

            // Create Orders table with Foreign Key)
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: false),
                    // Foreign Key Column
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<long>(type: "INTEGER", nullable: false),
                    Price = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Create Index for FK for faster query
            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                table: "Orders",
                column: "ProductId");


            //  DATA MIGRATION & SEEDING 
           
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[,]
                {
                { 1, "Widget", 12.99 },
                { 2, "Gadget", 15.49 },
                { 3, "Doohickey", 8.75 }
                });

            // Migrate Legacy Data

            // 1. Find the ProductName in the Backup table.
            // 2. If that name is NOT yet in the new Products table,
            //    then automatically create a new Product so that the order does not have a FK error.
            migrationBuilder.Sql($@"
                INSERT INTO Products (Name, Price)
                SELECT DISTINCT b.ProductName, 0 
                FROM {backupTableName} b
                WHERE b.ProductName IS NOT NULL 
                AND NOT EXISTS (SELECT 1 FROM Products p WHERE p.Name = b.ProductName);
            ");

            // Convert Orders (Map Name -> ID)
            // Use a JOIN between the Backup table and the new Products table to find the ID
            migrationBuilder.Sql($@"
                INSERT INTO Orders (Id, CustomerName, ProductId, Quantity, Price)
                SELECT 
                    b.Id,
                    COALESCE(b.CustomerName, 'Unknown'),
                
                    -- MAP TÊN SANG ID
                    p.Id, 
                
                    COALESCE(b.Quantity, 0),
                    COALESCE(b.Price, 0)
                FROM {backupTableName} b
                JOIN Products p ON b.ProductName = p.Name;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Orders");
            migrationBuilder.DropTable(name: "Products");
        }
    }
}
