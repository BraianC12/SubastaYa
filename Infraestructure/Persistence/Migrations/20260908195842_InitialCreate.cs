using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password_Hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha_Registro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auditorias_Log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Entidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entidad_Id = table.Column<int>(type: "int", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Detalle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usuario_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias_Log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auditorias_Log_Usuarios_Usuario_Id",
                        column: x => x.Usuario_Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Billeteras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Saldo_Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Saldo_Retenido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Saldo_Disponible = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Usuario_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Billeteras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Billeteras_Usuarios_Usuario_Id",
                        column: x => x.Usuario_Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subastas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url_Imagen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Precio_Base = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Incremento_Minimo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha_Inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fecha_Fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Vendedor_Id = table.Column<int>(type: "int", nullable: false),
                    Categoria_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subastas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subastas_Categorias_Categoria_Id",
                        column: x => x.Categoria_Id,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subastas_Usuarios_Vendedor_Id",
                        column: x => x.Vendedor_Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pujas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha_Puja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comprador_Id = table.Column<int>(type: "int", nullable: false),
                    Subasta_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pujas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pujas_Subastas_Subasta_Id",
                        column: x => x.Subasta_Id,
                        principalTable: "Subastas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pujas_Usuarios_Comprador_Id",
                        column: x => x.Comprador_Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transacciones_Ledger",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Subasta_Id = table.Column<int>(type: "int", nullable: true),
                    Billetera_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacciones_Ledger", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transacciones_Ledger_Billeteras_Billetera_Id",
                        column: x => x.Billetera_Id,
                        principalTable: "Billeteras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transacciones_Ledger_Subastas_Subasta_Id",
                        column: x => x.Subasta_Id,
                        principalTable: "Subastas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Tecnología" },
                    { 2, "Coleccionables" },
                    { 3, "Indumentaria" },
                    { 4, "Vehículos" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "Fecha_Registro", "Nombre", "Password_Hash" },
                values: new object[,]
                {
                    { 1, "vendedor@test.com", new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Vendedor", "hash1" },
                    { 2, "comprador1@test.com", new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Comprador Lider", "hash2" },
                    { 3, "comprador2@test.com", new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Comprador Habilitado", "hash3" },
                    { 4, "sinfondos@test.com", new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Usuario Sin Fondos", "hash4" }
                });

            migrationBuilder.InsertData(
                table: "Billeteras",
                columns: new[] { "Id", "Saldo_Disponible", "Saldo_Retenido", "Saldo_Total", "Usuario_Id", "Version" },
                values: new object[,]
                {
                    { 1, 0m, 0m, 0m, 1, 1 },
                    { 2, 105000m, 45000m, 150000m, 2, 1 },
                    { 3, 200000m, 0m, 200000m, 3, 1 },
                    { 4, 500m, 0m, 500m, 4, 1 }
                });

            migrationBuilder.InsertData(
                table: "Subastas",
                columns: new[] { "Id", "Categoria_Id", "Descripcion", "Estado", "Fecha_Fin", "Fecha_Inicio", "Incremento_Minimo", "Precio_Base", "Titulo", "Url_Imagen", "Vendedor_Id", "Version" },
                values: new object[,]
                {
                    { 1, 1, "Activa Estandar", "ACTIVA", new DateTime(2026, 10, 1, 12, 25, 0, 0, DateTimeKind.Utc), new DateTime(2026, 10, 1, 11, 0, 0, 0, DateTimeKind.Utc), 500m, 10000m, "MacBook Pro", "macbook.jpg", 1, 1 },
                    { 2, 2, "Activa Crítica", "ACTIVA", new DateTime(2026, 10, 1, 12, 1, 0, 0, DateTimeKind.Utc), new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), 100m, 5000m, "Reloj Antiguo", "reloj.jpg", 1, 1 },
                    { 3, 3, "Próxima", "PROGRAMADA", new DateTime(2026, 10, 3, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 10, 2, 12, 0, 0, 0, DateTimeKind.Utc), 1000m, 20000m, "Campera Cuero", "campera.jpg", 1, 1 },
                    { 4, 4, "Vencida con ganador", "ACTIVA", new DateTime(2026, 9, 30, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 26, 12, 0, 0, 0, DateTimeKind.Utc), 5000m, 100000m, "Moto Honda", "moto.jpg", 1, 1 },
                    { 5, 1, "Vencida desierta", "ACTIVA", new DateTime(2026, 9, 29, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 28, 12, 0, 0, 0, DateTimeKind.Utc), 1000m, 30000m, "Monitor 4K", "monitor.jpg", 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "Pujas",
                columns: new[] { "Id", "Comprador_Id", "Fecha_Puja", "Monto", "Subasta_Id" },
                values: new object[,]
                {
                    { 1, 3, new DateTime(2026, 10, 1, 11, 30, 0, 0, DateTimeKind.Utc), 25000m, 1 },
                    { 2, 2, new DateTime(2026, 10, 1, 11, 50, 0, 0, DateTimeKind.Utc), 45000m, 1 },
                    { 3, 3, new DateTime(2026, 9, 29, 12, 0, 0, 0, DateTimeKind.Utc), 120000m, 4 }
                });

            migrationBuilder.InsertData(
                table: "Transacciones_Ledger",
                columns: new[] { "Id", "Billetera_Id", "Fecha", "Monto", "Subasta_Id", "Tipo" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2026, 9, 21, 12, 0, 0, 0, DateTimeKind.Utc), 150000m, null, "DEPOSITO" },
                    { 2, 3, new DateTime(2026, 9, 21, 12, 0, 0, 0, DateTimeKind.Utc), 200000m, null, "DEPOSITO" },
                    { 3, 4, new DateTime(2026, 9, 21, 12, 0, 0, 0, DateTimeKind.Utc), 500m, null, "DEPOSITO" },
                    { 4, 2, new DateTime(2026, 10, 1, 11, 50, 0, 0, DateTimeKind.Utc), 45000m, 1, "RETENCION" },
                    { 5, 3, new DateTime(2026, 9, 29, 12, 0, 0, 0, DateTimeKind.Utc), 120000m, 4, "RETENCION" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_Log_Usuario_Id",
                table: "Auditorias_Log",
                column: "Usuario_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Billeteras_Usuario_Id",
                table: "Billeteras",
                column: "Usuario_Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pujas_Comprador_Id",
                table: "Pujas",
                column: "Comprador_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Pujas_Subasta_Id",
                table: "Pujas",
                column: "Subasta_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Subastas_Categoria_Id",
                table: "Subastas",
                column: "Categoria_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Subastas_Vendedor_Id",
                table: "Subastas",
                column: "Vendedor_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_Ledger_Billetera_Id",
                table: "Transacciones_Ledger",
                column: "Billetera_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_Ledger_Subasta_Id",
                table: "Transacciones_Ledger",
                column: "Subasta_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auditorias_Log");

            migrationBuilder.DropTable(
                name: "Pujas");

            migrationBuilder.DropTable(
                name: "Transacciones_Ledger");

            migrationBuilder.DropTable(
                name: "Billeteras");

            migrationBuilder.DropTable(
                name: "Subastas");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
