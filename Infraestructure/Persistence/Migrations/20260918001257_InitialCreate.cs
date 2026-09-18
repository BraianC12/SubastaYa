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
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
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
                    Estado = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
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
                    { 1, "Tecnologia" },
                    { 2, "Coleccionables" },
                    { 3, "Indumentaria" },
                    { 4, "Vehiculos" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "Fecha_Registro", "Nombre", "Password_Hash" },
                values: new object[,]
                {
                    { 1, "vendedor@test.com", new DateTime(2026, 8, 18, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), "Vendedor", "$2y$10$TodzyNai9KReU03EQu46Y.OG28bJbBSupc2PT8BXgeTbINfVWEJuK" },
                    { 2, "comprador1@test.com", new DateTime(2026, 8, 28, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), "Comprador Lider", "$2y$10$TodzyNai9KReU03EQu46Y.OG28bJbBSupc2PT8BXgeTbINfVWEJuK" },
                    { 3, "comprador2@test.com", new DateTime(2026, 9, 2, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), "Comprador Habilitado", "$2y$10$TodzyNai9KReU03EQu46Y.OG28bJbBSupc2PT8BXgeTbINfVWEJuK" },
                    { 4, "sinfondos@test.com", new DateTime(2026, 9, 7, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), "Usuario Sin Fondos", "$2y$10$TodzyNai9KReU03EQu46Y.OG28bJbBSupc2PT8BXgeTbINfVWEJuK" }
                });

            migrationBuilder.InsertData(
                table: "Billeteras",
                columns: new[] { "Id", "Saldo_Disponible", "Saldo_Retenido", "Saldo_Total", "Usuario_Id" },
                values: new object[,]
                {
                    { 1, 0m, 0m, 0m, 1 },
                    { 2, 105000m, 45000m, 150000m, 2 },
                    { 3, 200000m, 0m, 200000m, 3 },
                    { 4, 500m, 0m, 500m, 4 }
                });

            migrationBuilder.InsertData(
                table: "Subastas",
                columns: new[] { "Id", "Categoria_Id", "Descripcion", "Estado", "Fecha_Fin", "Fecha_Inicio", "Incremento_Minimo", "Precio_Base", "Titulo", "Url_Imagen", "Vendedor_Id" },
                values: new object[,]
                {
                    { 1, 1, "Activa Estandar", "ACTIVA", new DateTime(2026, 9, 17, 21, 37, 57, 478, DateTimeKind.Local).AddTicks(9780), new DateTime(2026, 9, 17, 20, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), 500m, 10000m, "MacBook Pro", "https://imgs.search.brave.com/J6-wF1GTePlaf-q8ScW9AG59Iiwp5IWXjXc42fJ1rqg/rs:fit:500:0:1:0/g:ce/aHR0cHM6Ly93d3cu/YXBwbGUuY29tL3Yv/bWFjYm9vay1wcm8v/YXgvaW1hZ2VzL292/ZXJ2aWV3L3dlbGNv/bWUvaGVyb19lbmRm/cmFtZV9fZndldjll/Ymg0Mm1xX3hsYXJn/ZS5qcGc", 1 },
                    { 2, 2, "Activa Crítica", "ACTIVA", new DateTime(2026, 9, 17, 21, 13, 57, 478, DateTimeKind.Local).AddTicks(9780), new DateTime(2026, 9, 17, 19, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), 100m, 5000m, "Reloj Antiguo", "https://imgs.search.brave.com/wLx1K_lbAiBugpCVvNHXj4bv3w48P3iTTWjqGKbuNNg/rs:fit:500:0:1:0/g:ce/aHR0cHM6Ly9pLmVi/YXlpbWcuY29tL2lt/YWdlcy9nL3U0d0FB/T1N3YW5SWGcxYlEv/cy1sNDAwLndlYnA", 1 },
                    { 3, 3, "Próxima", "PROGRAMADA", new DateTime(2026, 9, 19, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), new DateTime(2026, 9, 18, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), 1000m, 20000m, "Campera Cuero", "https://imgs.search.brave.com/78zXlb1RFASO9v8TmGTyEHbjuQV3Z4Pb2jr9Y1A6mlA/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly9odHRw/Mi5tbHN0YXRpYy5j/b20vRF9OUV9OUF82/NDA3NTEtTUxBNTEz/NDY3MjgyMTZfMDgy/MDIyLVcud2VicA", 1 },
                    { 4, 4, "Vencida con ganador", "FINALIZADA", new DateTime(2026, 9, 16, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), new DateTime(2026, 9, 12, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), 5000m, 100000m, "Moto Honda", "https://imgs.search.brave.com/OwjFjSvv0S8ohAmt395Fx2GCJChEk7U13LAuRYJt_lU/rs:fit:500:0:1:0/g:ce/aHR0cHM6Ly9zY2Fs/ZXRodW1iLmxlcGFy/a2luZy5mci91bnNh/ZmUvMzAweDIyNS9o/dHRwczovL2Nsb3Vk/LmxlcGFya2luZy1t/b3RvLmZyLzIwMjUv/MTAvMTAvMDcvMTEv/aG9uZGEtbmVzLW1v/dG9ycmFkLWhvbmRh/LW5lcy0xMjVfMjUz/OTU0NDY4LmpwZw", 1 },
                    { 5, 1, "Vencida desierta", "DESIERTA", new DateTime(2026, 9, 15, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), new DateTime(2026, 9, 13, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), 1000m, 30000m, "Monitor 4K", "https://imgs.search.brave.com/GxxLh9QfgiHzyAdfGJdwmlCY3HDs81brY9Rr58XEfGI/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly9tLm1l/ZGlhLWFtYXpvbi5j/b20vaW1hZ2VzL0kv/NjFlWFdSeGd4Skwu/anBn", 1 },
                    { 6, 1, "Vence el Sábado para probar pujas", "ACTIVA", new DateTime(2026, 9, 21, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), new DateTime(2026, 9, 17, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), 2000m, 80000m, "Smart TV Samsung 55'", "https://imgs.search.brave.com/jJ3FtIEDfioe0Qx2YFJLbqkKmZJ_xxLZQjm6zW1P3b0/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly9tLm1l/ZGlhLWFtYXpvbi5j/b20vaW1hZ2VzL0kv/OTFibFNpckVqT0wu/anBn", 1 },
                    { 7, 1, "Vence el Domingo para probar pujas", "ACTIVA", new DateTime(2026, 9, 22, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), new DateTime(2026, 9, 17, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), 3000m, 120000m, "PlayStation 5", "https://imgs.search.brave.com/TZ19Apjni8mGzUZkWey-zYtFy4VZ6lDLhLxrWhSQesQ/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly93d3cu/ZW5nYWRnZXQuY29t/L2ltZy9nYWxsZXJ5/L2hvdy1sb25nLWNh/bi15b3UtZXhwZWN0/LWEtcGxheXN0YXRp/b24tNS10by1sYXN0/L2ludHJvLTE3ODUx/NTQ5MDUuanBn", 1 }
                });

            migrationBuilder.InsertData(
                table: "Pujas",
                columns: new[] { "Id", "Comprador_Id", "Fecha_Puja", "Monto", "Subasta_Id" },
                values: new object[,]
                {
                    { 1, 3, new DateTime(2026, 9, 17, 20, 32, 57, 478, DateTimeKind.Local).AddTicks(9780), 25000m, 1 },
                    { 2, 2, new DateTime(2026, 9, 17, 21, 2, 57, 478, DateTimeKind.Local).AddTicks(9780), 45000m, 1 },
                    { 3, 3, new DateTime(2026, 9, 15, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), 120000m, 4 }
                });

            migrationBuilder.InsertData(
                table: "Transacciones_Ledger",
                columns: new[] { "Id", "Billetera_Id", "Fecha", "Monto", "Subasta_Id", "Tipo" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2026, 9, 12, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), 150000m, null, "DEPOSITO" },
                    { 2, 3, new DateTime(2026, 9, 12, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), 200000m, null, "DEPOSITO" },
                    { 3, 4, new DateTime(2026, 9, 12, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), 500m, null, "DEPOSITO" },
                    { 4, 2, new DateTime(2026, 9, 17, 21, 2, 57, 478, DateTimeKind.Local).AddTicks(9780), 45000m, 1, "RETENCION" },
                    { 5, 3, new DateTime(2026, 9, 16, 21, 12, 57, 478, DateTimeKind.Local).AddTicks(9780), 120000m, 4, "DEBITO" }
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
                name: "IX_Pujas_Subasta_Id_Monto",
                table: "Pujas",
                columns: new[] { "Subasta_Id", "Monto" });

            migrationBuilder.CreateIndex(
                name: "IX_Subastas_Categoria_Id",
                table: "Subastas",
                column: "Categoria_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Subastas_Estado_Categoria_Id_Fecha_Fin",
                table: "Subastas",
                columns: new[] { "Estado", "Categoria_Id", "Fecha_Fin" })
                .Annotation("SqlServer:Include", new[] { "Titulo", "Precio_Base" });

            migrationBuilder.CreateIndex(
                name: "IX_Subastas_Fecha_Fin",
                table: "Subastas",
                column: "Fecha_Fin",
                filter: "[Estado] = 'ACTIVA'");

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

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
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
